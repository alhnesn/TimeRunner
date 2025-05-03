using UnityEngine;

public class ChaserMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float jumpHeight = 1.5f; // Desired jump height in meters
    private float jumpForce;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpForce = Mathf.Sqrt(2f * jumpHeight * -Physics2D.gravity.y * rb.gravityScale);
    }

    void FixedUpdate()
    {
        // Constant horizontal movement
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Raycast ahead to detect obstacle
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, obstacleLayer);
        if (hit.collider != null && hit.collider.CompareTag("Obstacle") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Update()
    {
        // Optional: debug ray (Scene view only)
        Debug.DrawRay(transform.position, Vector2.right * 1.5f, Color.red);
    }
}
