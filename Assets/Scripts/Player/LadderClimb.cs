using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 5f;
    private bool isClimbing = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
            rb.gravityScale = 0f; // Disable gravity while climbing
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 1f; // Reset gravity after leaving ladder
        }
    }
}
