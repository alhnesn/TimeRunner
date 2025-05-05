// ProjectileSpawner.cs
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject projectilePrefab;

    [Header("Player")]
    public PlayerController playerController;

    [Header("Spawn Timing (seconds)")]
    [Tooltip("Starting seconds between waves")]
    public float initialSpawnInterval = 2f;
    [Tooltip("Minimum seconds between waves, no matter how far you run")]
    public float minSpawnInterval = 0.5f;
    [Tooltip("How much to reduce interval per unit X distance")]
    public float intervalDecreasePerUnit = 0.005f;

    [Tooltip("Seconds ahead of camera edge to spawn")]
    public float spawnLeadTime = 2f;

    private float spawnInterval;
    private float timer;

    void Start()
    {
        spawnInterval = initialSpawnInterval;
        timer = spawnInterval;
    }

    void Update()
    {
        if (RewindManager.I.IsRewinding) return;

        // 2) dynamically shorten spawnInterval as player runs
        float distance = playerController.transform.position.x;
        spawnInterval = Mathf.Max(
            minSpawnInterval,
            initialSpawnInterval - distance * intervalDecreasePerUnit
        );

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnWave();
            timer = spawnInterval;
        }
    }

    void SpawnWave()
    {
        if (projectilePrefab == null || playerController == null) return;

        // 1) find camera right edge
        Camera cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float cameraRightX = cam.transform.position.x + halfW;

        // 2) lead distance = player maxSpeed * leadTime
        float leadDist = playerController.moveSpeed * spawnLeadTime;
        float spawnX = cameraRightX + leadDist;

        // 3) full vertical range of camera
        float cameraCenterY = cam.transform.position.y;
        float spawnMinY = cameraCenterY - halfH;
        float spawnMaxY = cameraCenterY + halfH;

        // spawn 1 projectile this wave
        Vector3 pos = new Vector3(
            spawnX,
            Random.Range(spawnMinY, spawnMaxY),
            0f
        );
        Instantiate(projectilePrefab, pos, Quaternion.identity);
    }
}
