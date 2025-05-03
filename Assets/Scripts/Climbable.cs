using UnityEngine;

public class Climbable : MonoBehaviour
{
    public float climbSpeed = 3f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKey(KeyCode.W))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeed);
        }
    }
}
