//using UnityEngine;

//public class Chaser : MonoBehaviour
//{
//    public Transform player;
//    public float speed = 4f;

//    void Update()
//    {
//        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
//    }
//}


using UnityEngine;
public class Chaser : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float jumpHeight = 1.5f; // Desired jump height in Unity units (meters)
    private float jumpForce;        // Will be calculated from jumpHeight
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Calculate jump force using the correct formula
        // This formula gives a more accurate jump height
        jumpForce = Mathf.Sqrt(2f * jumpHeight * -Physics2D.gravity.y * rb.gravityScale);
    }

    void Update()
    {
        // If you're on Unity 6 preview and see warning, try this:
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // Ground check using overlap circle at the feet
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Visual ray for debugging
        Debug.DrawRay(transform.position, Vector2.right * 1.5f, Color.red);

        // Raycast ahead to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, obstacleLayer);
        if (hit.collider != null && hit.collider.CompareTag("Obstacle") && isGrounded)
        {
            // Apply upward velocity directly instead of impulse force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

    }
}
