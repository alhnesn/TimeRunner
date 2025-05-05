using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject ingameUI;
    public GameObject player;
    public GameObject chaser;
    public GameObject rocket;
    private bool gameStarted=false;

    public void Start()
    {
        
        mainMenuUI.SetActive(true);
        ingameUI.SetActive(false);
        player.SetActive(false);
        chaser.SetActive(false);
        rocket.SetActive(false);


    }

    public void Update()
    {
        if (!gameStarted&&(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.A)))
{
    gameStarted=true;
        player.SetActive(true);
        chaser.SetActive(true);
        rocket.SetActive(true);
        mainMenuUI.SetActive(false);
        ingameUI.SetActive(true);


}
    }
    public void GameOver()
    {
        ScoreManagerTMP.I.OnPlayerDeath();


        Debug.Log("YAKALANDIN!");
        // Buraya sahne reset, animasyon veya menü açma eklenebilir
        Time.timeScale = 0f;
    }



    
}
