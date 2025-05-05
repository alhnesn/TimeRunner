using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHazard : MonoBehaviour
{
    [Tooltip("Reference to your GameManager")]
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            if (gameManager != null)
                gameManager.GameOver();
            else
                Debug.LogWarning("PlayerHazard: GameManager not assigned!");
        }
    }
}
