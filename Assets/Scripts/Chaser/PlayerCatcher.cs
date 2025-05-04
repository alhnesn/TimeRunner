using UnityEngine;

public class PlayerCatcher : MonoBehaviour
{

    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player caught!");
            gameManager.GameOver();
      
        }
    }
}
