using UnityEngine;

public class PlayerCatchCheck : MonoBehaviour
{
    public GameObject chaser;
    public GameManager gameManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            gameManager.GameOver();
        }
    }
}
