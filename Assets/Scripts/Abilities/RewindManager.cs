// RewindManager.cs
using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    public static RewindManager I { get; private set; }

    [Header("Rewind Settings")]
    [Tooltip("How many seconds of history to keep for all rewindables.")]
    public float bufferDuration = 5f;
    [Tooltip("Speed multiplier when rewinding (e.g. 2 = twice as fast).")]
    public float rewindSpeed = 2f;
    [Tooltip("Maximum seconds of rewind allowed when the bar is full.")]
    public float rewindLimit = 2f;
    [Tooltip("How many seconds it takes to fully refill the rewind bar.")]
    public float rechargeTime = 20f;

    // Current available rewind “energy” (0…rewindLimit)
    float currentCharge = 0f;

    // Internal timeline cursor
    float playTime = 0f;
    bool isRewinding = false;

    List<IRewindable> rewindables = new List<IRewindable>();

    /// <summary>True if we’re currently in rewind mode.</summary>
    public bool IsRewinding => isRewinding;

    /// <summary>How much rewind time you currently have (0…rewindLimit).</summary>
    public float RewindCharge => currentCharge;

    /// <summary>How much rewind time you get when the bar is full.</summary>
    public float RewindLimit => rewindLimit;

    void Awake()
    {
        if (I != null) Destroy(gameObject);
        else I = this;
        currentCharge = 0f;
    }

    public void Register(IRewindable r) => rewindables.Add(r);
    public void Unregister(IRewindable r) => rewindables.Remove(r);

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        if (!isRewinding)
        {
            // Advance timeline and record history
            playTime += dt;
            foreach (var r in rewindables)
                r.RecordState(playTime);

            // Recharge bar (up to limit)
            float refillRate = rewindLimit / rechargeTime;
            currentCharge = Mathf.Min(currentCharge + dt * refillRate, rewindLimit);
        }
        else
        {
            // How much time we actually rewind this step
            float dtRewound = dt * rewindSpeed;

            playTime -= dtRewound;
            currentCharge -= dtRewound;

            // Clamp and possibly stop
            if (playTime <= 0f || currentCharge <= 0f)
            {
                playTime = Mathf.Max(playTime, 0f);
                currentCharge = Mathf.Max(currentCharge, 0f);
                StopRewind();
                return;
            }

            // Restore all rewindables
            foreach (var r in rewindables)
                r.RestoreState(playTime);
        }
    }

    /// <summary>
    /// Begin rewinding only if we have full charge and some history.
    /// </summary>
    public void StartRewind()
    {
        if (playTime > 0f && currentCharge >= rewindLimit)
            isRewinding = true;
    }

    /// <summary>Stop rewinding immediately.</summary>
    public void StopRewind() => isRewinding = false;
}
