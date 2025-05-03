using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f; // Desired jump height in meters
    private float jumpForce;

    private int jumpCount = 0;
    public int maxJumps = 2;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump = false;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private float moveInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpForce = Mathf.Sqrt(2f * jumpHeight * -Physics2D.gravity.y * rb.gravityScale);
    }

    void Update()
    {
        // Get left/right input (-1, 0, or 1)
        moveInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows

        // Handle jump input
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            shouldJump = true;
        }
    }

    void FixedUpdate()
    {
        // Move horizontally based on input
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (shouldJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            shouldJump = false;
        }
    }
}
