using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RewindableRigidbody : MonoBehaviour, IRewindable
{
    struct State
    {
        public float time;
        public Vector3 pos;
        public Quaternion rot;
        public Vector2 velocity;
        public bool active;
    }

    [Tooltip("How many seconds to buffer for rewind")]
    public float bufferDuration = 5f;

    List<State> history = new List<State>();
    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void RecordState(float t)
    {
        // Capture
        history.Add(new State
        {
            time = t,
            pos = transform.position,
            rot = transform.rotation,
            velocity = rb.linearVelocity,
            active = gameObject.activeSelf
        });
        // Trim older than bufferDuration
        while (history.Count > 0 && t - history[0].time > bufferDuration)
            history.RemoveAt(0);
    }

    public void RestoreState(float rewindToTime)
    {
        // Find the latest recorded state ≤ rewindToTime
        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].time <= rewindToTime)
            {
                var s = history[i];
                transform.position = s.pos;
                transform.rotation = s.rot;
                rb.linearVelocity = s.velocity;
                gameObject.SetActive(s.active);
                return;
            }
        }
    }
}
