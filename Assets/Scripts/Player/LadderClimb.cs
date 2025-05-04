//using UnityEngine;

//public class LadderClimb : MonoBehaviour
//{
//    public float climbSpeed = 5f;
//    private bool isClimbing = false;
//    private Rigidbody2D rb;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//    }

//    void Update()
//    {
//        if (isClimbing)
//        {
//            float verticalInput = Input.GetAxis("Vertical");

//            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
//            rb.gravityScale = 0f; // Disable gravity while climbing
//        }
//    }

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Ladder"))
//        {
//            isClimbing = true;
//        }
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Ladder"))
//        {
//            isClimbing = false;
//            rb.gravityScale = 1f; // Reset gravity after leaving ladder
//        }
//    }
//}


using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LadderClimb : MonoBehaviour
{
    [Header("Climbing")]
    [Tooltip("Speed at which the player climbs the ladder.")]
    public float climbSpeed = 5f;

    [Header("Debug")]
    [Tooltip("Whether the player is currently in any ladder zone.")]
    [SerializeField] private bool inClimbZone = false;
    [Tooltip("Whether the player is actively climbing.")]
    [SerializeField] private bool isClimbing = false;
    [Tooltip("Number of overlapping ladder colliders.")]
    [SerializeField] private int ladderContactCount = 0;
    [Tooltip("Current vertical input value.")]
    [SerializeField] private float verticalInput = 0f;
    [Tooltip("Current horizontal input value.")]
    [SerializeField] private float horizontalInput = 0f;

    private Rigidbody2D rb;
    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Read debug horizontal input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Only process climb logic if overlapping at least one ladder collider
        if (ladderContactCount <= 0)
            return;

        verticalInput = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(verticalInput) > 0f)
        {
            if (!isClimbing)
                BeginClimb();

            // Preserve horizontal velocity, adjust vertical
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Allow Y movement
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
        }
        else if (isClimbing)
        {
            // Hold vertical position, allow horizontal movement
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            ladderContactCount++;
            inClimbZone = ladderContactCount > 0;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            ladderContactCount = Mathf.Max(0, ladderContactCount - 1);
            inClimbZone = ladderContactCount > 0;

            // Only end climbing when exiting all ladder colliders
            if (!inClimbZone && isClimbing)
                EndClimb();
        }
    }

    /// <summary>
    /// Enables climbing: disable gravity, keep horizontal free.
    /// </summary>
    private void BeginClimb()
    {
        isClimbing = true;
        rb.gravityScale = 0f;
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.linearVelocity = Vector2.zero;
        // Only lock rotation, horizontal movement is managed by player
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Restores normal physics when leaving the ladder.
    /// </summary>
    private void EndClimb()
    {
        isClimbing = false;
        rb.gravityScale = originalGravity;
        // Reset constraints to only freeze rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}

