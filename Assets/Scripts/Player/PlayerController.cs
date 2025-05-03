using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f; // Desired jump height in meters
    private float jumpForce;

    private int jumpCount = 0;
    public int maxJumps = 2; // 1 means max 2 jumps total

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump = false;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculate jump force using physics formula
        jumpForce = Mathf.Sqrt(2f * jumpHeight * -Physics2D.gravity.y * rb.gravityScale);
    }

    void Update()
    {
        // Handle input in Update
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            shouldJump = true;
        }
    }

    void FixedUpdate()
    {
        // Constant movement to the right
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset jump count when grounded
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // Apply jump in FixedUpdate
        if (shouldJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            shouldJump = false;
        }
    }
}
