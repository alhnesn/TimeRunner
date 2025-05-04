using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    public static RewindManager I { get; private set; }
    [Header("Rewind Settings")]
    [Tooltip("How many seconds of history to keep for all rewindables.")]
    public float bufferDuration = 5f;
    public float rewindSpeed = 2f;
    float playTime = 0f;
    bool isRewinding = false;
    List<IRewindable> rewindables = new List<IRewindable>();

    public bool IsRewinding => isRewinding;    // expose for others
    void Awake()
    {
        if (I != null) Destroy(gameObject);
        else I = this;
    }

    public void Register(IRewindable r) => rewindables.Add(r);
    public void Unregister(IRewindable r) => rewindables.Remove(r);

    void FixedUpdate()
    {
        if (!isRewinding)
        {
            playTime += Time.fixedDeltaTime;
            foreach (var r in rewindables) r.RecordState(playTime);
        }
        else
        {
            playTime -= Time.fixedDeltaTime * rewindSpeed;
            if (playTime <= 0f)
            {
                // ran out of history → stop
                playTime = 0f;
                StopRewind();
                return;
            }
            foreach (var r in rewindables) r.RestoreState(playTime);
        }
    }

    public void StartRewind()
    {
        if (playTime > 0f) isRewinding = true;
    }

    public void StopRewind() => isRewinding = false;
}
