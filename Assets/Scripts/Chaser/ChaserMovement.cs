//using UnityEngine;

//public class ChaserMovement : MonoBehaviour
//{
//    public float moveSpeed = 4f;
//    public float jumpHeight = 1.5f; // Desired jump height in meters
//    private float jumpForce;

//    public Transform groundCheck;
//    public LayerMask groundLayer;
//    public LayerMask obstacleLayer;

//    private Rigidbody2D rb;
//    private bool isGrounded;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        jumpForce = Mathf.Sqrt(2f * jumpHeight * -Physics2D.gravity.y * rb.gravityScale);
//    }

//    void FixedUpdate()
//    {
//        // Constant horizontal movement
//        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

//        // Ground check
//        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

//        // Raycast ahead to detect obstacle
//        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, obstacleLayer);
//        if (hit.collider != null && hit.collider.CompareTag("Obstacle") && isGrounded)
//        {
//            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//        }
//    }

//    void Update()
//    {
//        // Optional: debug ray (Scene view only)
//        Debug.DrawRay(transform.position, Vector2.right * 1.5f, Color.red);
//    }
//}


using System.Collections.Generic;
using UnityEngine;

public class ChaserMovement : MonoBehaviour, IRewindable
{
    [Header("Chase Settings")]
    [Tooltip("Target to chase (the player)")]
    public Transform target;
    [Tooltip("Movement speed")]
    public float speed = 4f;
    [Tooltip("Whether to chase in X axis only (useful for side-scrollers)")]
    public bool horizontalOnly = false;
    [Tooltip("Maximum distance allowed between chaser and target")]
    public float maxDistance = 10f;

    // ─────────── Rewindable State ───────────
    struct State
    {
        public float time;
        public Vector3 position;
    }
    private List<State> history = new List<State>();

    void OnEnable()
    {
        RewindManager.I.Register(this);
    }

    void OnDisable()
    {
        RewindManager.I.Unregister(this);
    }

    /// <summary>
    /// Called by RewindManager every FixedUpdate during normal play.
    /// </summary>
    public void RecordState(float t)
    {
        history.Add(new State
        {
            time = t,
            position = transform.position
        });

        // Trim entries older than the global bufferDuration
        float buf = RewindManager.I.bufferDuration;
        while (history.Count > 0 && t - history[0].time > buf)
            history.RemoveAt(0);
    }

    /// <summary>
    /// Called by RewindManager every FixedUpdate during rewind.
    /// Restores the chaser’s position to what it was at time t.
    /// </summary>
    public void RestoreState(float t)
    {
        // Find the latest recorded state at or before t
        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].time <= t)
            {
                transform.position = history[i].position;
                return;
            }
        }
        // If no history exists (e.g. rewound to t=0), you might choose to do nothing
    }

    void Update()
    {
        // Don’t run normal chase logic while rewinding
        if (RewindManager.I.IsRewinding)
            return;

        if (target == null)
            return;

        // Compute desired target position
        Vector3 targetPos = target.position;
        if (horizontalOnly)
            targetPos = new Vector3(target.position.x, transform.position.y, transform.position.z);

        // Enforce max distance
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > maxDistance)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position = targetPos - dir * maxDistance;
        }

        // Move toward the (possibly clamped) target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }
}

