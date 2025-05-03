using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    private bool isTouchingPlayer = false;

    // Player kutuya çarptığında, kutu yok olacak
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
        else if (collision.collider.CompareTag("Chaser"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // D tuşuna basıldığında kutu yok olacak
        if (isTouchingPlayer && Input.GetKeyDown(KeyCode.D))
        {
            Destroy(gameObject);
        }
    }

    // Player kutudan ayrıldığında, yok olma işlemi duracak
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }
}
