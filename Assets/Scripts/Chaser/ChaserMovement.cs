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


using UnityEngine;

public class ChaserMovement : MonoBehaviour
{
    [Tooltip("Target to chase (the player)")]
    public Transform target;

    [Tooltip("Movement speed")]
    public float speed = 4f;

    [Tooltip("Whether to chase in X axis only (useful for side-scrollers)")]
    public bool horizontalOnly = false;

    [Tooltip("Maximum distance allowed between chaser and target")]
    public float maxDistance = 10f;

    void Update()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;

        if (horizontalOnly)
            targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);

        // Enforce max distance
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > maxDistance)
        {
            // Snap closer (you can lerp instead of teleport if preferred)
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position = targetPosition - direction * maxDistance;
        }

        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
