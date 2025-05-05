using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
public class Projectile : MonoBehaviour, IRewindable
{
    [Header("Movement")]
    [Tooltip("Leftward speed")]
    public float speed = 5f;

    [Header("Despawn")]
    [Tooltip("Seconds after leaving camera before despawning")]
    public float destroyDelay = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private bool isActive = true;
    private bool hasLeftCamera = false;
    private float offscreenTimer = 0f;

    // State snapshot for rewind
    private struct State
    {
        public float time;
        public Vector2 pos, vel;
        public bool isActive, leftCamera;
        public float offscreenTimer;
    }
    private List<State> history = new List<State>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Kinematic for controlled movement
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void OnEnable()
    {
        if (RewindManager.I != null)
            RewindManager.I.Register(this);
    }

    void OnDisable()
    {
        if (RewindManager.I != null)
            RewindManager.I.Unregister(this);
    }

    // IRewindable ▶ record current snapshot
    public void RecordState(float t)
    {
        history.Add(new State
        {
            time = t,
            pos = rb.position,
            vel = rb.linearVelocity,
            isActive = isActive,
            leftCamera = hasLeftCamera,
            offscreenTimer = offscreenTimer
        });

        float buf = RewindManager.I.bufferDuration;
        while (history.Count > 0 && t - history[0].time > buf)
            history.RemoveAt(0);
    }

    // IRewindable ▶ restore to the snapshot at or before t
    public void RestoreState(float t)
    {
        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].time <= t)
            {
                var s = history[i];
                // physics
                rb.position = s.pos;
                rb.linearVelocity = s.vel;
                // logic
                isActive = s.isActive;
                hasLeftCamera = s.leftCamera;
                offscreenTimer = s.offscreenTimer;
                // visuals / collisions
                sr.enabled = s.isActive;
                col.enabled = s.isActive;
                return;
            }
        }
    }

    void FixedUpdate()
    {
        // Skip both when rewinding and when already “dead”
        if (RewindManager.I.IsRewinding || !isActive)
            return;

        // 1) Move left
        rb.linearVelocity = Vector2.left * speed;

        // 2) Detect leaving camera
        Camera cam = Camera.main;
        float leftX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;

        if (!hasLeftCamera && rb.position.x < leftX)
        {
            hasLeftCamera = true;
            offscreenTimer = 0f;
        }

        // 3) After delay, “despawn” (disable) to support rewind
        if (hasLeftCamera)
        {
            offscreenTimer += Time.fixedDeltaTime;
            if (offscreenTimer >= destroyDelay)
                Deactivate();
        }
    }

    // Disables movement, sprite, collider—but keeps object alive for rewind
    private void Deactivate()
    {
        isActive = false;
        rb.linearVelocity = Vector2.zero;
        sr.enabled = false;
        col.enabled = false;
    }
}
