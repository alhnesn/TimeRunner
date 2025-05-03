using UnityEngine;

public class PlayerCatchCheck : MonoBehaviour
{
    public GameObject chaser;
    public GameManager gameManager;
    private bool isGameOverTriggered = false;

    void Update()
    {
        if (!isGameOverTriggered && chaser.transform.position.x >= transform.position.x)
        {
            isGameOverTriggered = true;
            gameManager.GameOver();
        }
    }
}
