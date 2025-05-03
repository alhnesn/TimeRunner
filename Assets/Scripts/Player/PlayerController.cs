using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    private int jumpCount = 0;
    public int maxJumps = 2;

    private Rigidbody2D rb;
    private bool isGrounded;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Sürekli sağa doğru hareket et
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // Yere temas kontrolü
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0; // Yere değiyorsa zıplama hakkını sıfırla
        }

        // Zıplama (Space tuşu ya da Input Manager'daki "Jump")
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }
    }
}