using UnityEngine;


public class GameManager : MonoBehaviour
{
    public void GameOver()
    {
        ScoreManagerTMP.I.OnPlayerDeath();


        Debug.Log("YAKALANDIN!");
        // Buraya sahne reset, animasyon veya menü açma eklenebilir
        Time.timeScale = 0f;
    }
}
