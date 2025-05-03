using UnityEngine;

public class PlayerControllerEyll : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float climbSpeed = 5f;
    public float jumpForce = 12f;
    public int maxJumps = 1;

    private int jumpCount = 0;
    private Rigidbody2D rb;
    private bool isClimbing = false;
    private bool atLadder = false;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Player horizontal movement
          if (!isClimbing && !atLadder)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }

        // Climbing logic
        if (atLadder && Input.GetKey(KeyCode.W))
        {
            isClimbing = true;
            rb.linearVelocity = new Vector2(0, climbSpeed);  // Climbing vertically
            rb.gravityScale = 0f;  // Disable gravity while climbing
        }

        if (isClimbing && Input.GetKeyUp(KeyCode.W))
        {
            isClimbing = false;
            rb.linearVelocity = Vector2.zero; // Stop when the player releases W
            rb.gravityScale = 2f; // Restore gravity
        }

        // Reset jump count if the player is grounded
        if (isGrounded)
        {
            jumpCount = 0; // Reset jump count when touching the ground
        }

        // Jump logic (Double Jump check)
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);  // Apply jump force
            jumpCount++; // Increase jump count
            isGrounded=false;
        }
    }

    // Using OnCollisionEnter2D and OnCollisionExit2D to detect ground with tags
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Reset jump count immediately when hitting ground
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ladder logic
        if (collision.CompareTag("Ladder"))
        {
            atLadder = true;
            rb.linearVelocity = Vector2.zero; // Stop player when touching ladder
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            atLadder = false;
            rb.gravityScale = 2f; // Restore gravity when leaving ladder
        }
    }
}