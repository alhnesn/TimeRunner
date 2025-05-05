using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    public static SpeedManager Instance { get; private set; }

    [Header("Speed Settings")]
    public float basePlayerSpeed = 8f;
    public float baseChaserSpeed = 4f;
    public float acceleration = 0.2f;             // Speed increase over time
    public float decelerationFactor = 0.9f;       // Applied on Tab press

    private float playerSpeed;
    private float chaserSpeed;

    public float GetPlayerSpeed() => playerSpeed;
    public float GetChaserSpeed() => chaserSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        playerSpeed = basePlayerSpeed;
        chaserSpeed = baseChaserSpeed;
    }

    private void Update()
    {
        // Gradual speed increase over time
        playerSpeed += acceleration * Time.deltaTime;
        chaserSpeed += acceleration * Time.deltaTime * 0.3f; // Optional: chaser increases slower
    }

    public void ReduceSpeedTemporarily()
    {
        playerSpeed *= decelerationFactor;
        chaserSpeed *= decelerationFactor;

        // Optional: clamp so it doesn't go too low
        playerSpeed = Mathf.Max(playerSpeed, basePlayerSpeed);
        chaserSpeed = Mathf.Max(chaserSpeed, baseChaserSpeed);
    }
}
