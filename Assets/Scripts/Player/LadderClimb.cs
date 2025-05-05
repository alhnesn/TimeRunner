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
    private Animator animator;

    private Rigidbody2D rb;
    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Read debug horizontal input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Only process climb logic if overlapping at least one ladder collider
        if (ladderContactCount <= 0)
        {
            isClimbing = false;
            animator?.SetBool("isClimbing", false);
            return;
        }

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

        animator?.SetBool("isClimbing", isClimbing);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
            ladderContactCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && --ladderContactCount <= 0)
            EndClimb();
    }

    /// <summary>
    /// Enables climbing: disable gravity, keep horizontal free.
    /// </summary>
    private void BeginClimb()
    {
        isClimbing = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Restores normal physics when leaving the ladder.
    /// </summary>
    private void EndClimb()
    {
        isClimbing = false;
        rb.gravityScale = originalGravity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}

