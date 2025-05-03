using UnityEngine;

public class BreakableBoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform playerTransform;
    public float spawnDistanceAhead = 15f;
    public float minY = -1f;
    public float maxY = 2f;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;
    public float overlapCheckRadius = 0.5f;
    public LayerMask obstacleLayer;

    private float timer = 0f;
    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            TrySpawnBox();
            timer = 0f;
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void TrySpawnBox()
    {
        float spawnX = playerTransform.position.x + spawnDistanceAhead;
        float spawnY = Random.Range(minY, maxY);
        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        if (!Physics2D.OverlapCircle(spawnPos, overlapCheckRadius, obstacleLayer))
        {
            Instantiate(boxPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("Skipped spawn due to overlap with obstacle.");
        }
    }
}
