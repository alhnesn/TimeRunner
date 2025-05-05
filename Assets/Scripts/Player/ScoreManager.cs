using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManagerTMP : MonoBehaviour
{
public static ScoreManagerTMP I;

[Header("UI References")]
public TMP_Text inGameScoreText;
public TMP_Text finalScoreText;
public TMP_Text highScoreText;
public GameObject deathPanel;

private float currentScore = 0f;
private Vector2 lastPosition;
private Transform playerTransform;

private const string HighScoreKey = "HighScore";

private void Awake()
{
    if (I != null && I != this)
    {
        Destroy(gameObject);
    }
    else
    {
        I = this;
       // DontDestroyOnLoad(gameObject);
    }
}

private void Start()
{
    playerTransform = GameObject.FindWithTag("Player")?.transform;

    if (playerTransform != null)
        lastPosition = playerTransform.position;

    UpdateScoreUI();
}

private void Update()
{
if (playerTransform == null) return;

Vector2 currentPos = playerTransform.position;
float deltaX = currentPos.x - lastPosition.x;

if (deltaX > 0f)
{
    currentScore += deltaX;
    UpdateScoreUI();
}

lastPosition = currentPos;

// ✅ Check for Retry key
if (deathPanel != null && deathPanel.activeSelf && Input.GetKeyDown(KeyCode.Q))
{
    Retry();
}


}
public void AddScore(int amount)
{
    currentScore += amount;
    UpdateScoreUI();
}


private void UpdateScoreUI()
{
    int roundedScore = Mathf.FloorToInt(currentScore);

    if (inGameScoreText)
        inGameScoreText.text = $"Score: {roundedScore}";

    if (finalScoreText)
        finalScoreText.text = $"Score: {roundedScore}";
}

// Called when the theme changes and player is repositioned
public void ThemeChangedResetPosition()
{
    if (playerTransform != null)
        lastPosition = playerTransform.position;
}

public void OnPlayerDeath()
{
    int roundedScore = Mathf.FloorToInt(currentScore);
    int best = PlayerPrefs.GetInt(HighScoreKey, 0);

    if (roundedScore > best)
    {
        best = roundedScore;
        PlayerPrefs.SetInt(HighScoreKey, best);
        PlayerPrefs.Save();
    }

    if (highScoreText)
        highScoreText.text = $"High Score: {best}";

    UpdateScoreUI(); // also updates finalScoreText

    if (deathPanel)
        deathPanel.SetActive(true);
}

public void Retry()
{
    SceneManager.LoadScene("Home");
    Time.timeScale=1f;
}

}
