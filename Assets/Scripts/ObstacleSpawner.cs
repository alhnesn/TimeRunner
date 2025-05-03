using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;        // Obstacle prefab'ları
    public Transform player;                    // Player referansı
    public float spawnDistanceAhead = 20f;      // Player'ın ne kadar ilerisinde spawn edilsin
    public float spawnSpacing = 10f;            // Her spawn grubunun ne kadar ilerisine spawn edilecek
    public float obstacleWidth = 1f;            // Obstacle'ların yatay genişliği

    private float nextSpawnX;

    void Start()
    {
        nextSpawnX = Mathf.Floor(player.position.x + spawnDistanceAhead);
    }

    void Update()
    {
        while (nextSpawnX < player.position.x + spawnDistanceAhead)
        {
            SpawnObstacleGroup(nextSpawnX);
            nextSpawnX += spawnSpacing;
        }
    }

    void SpawnObstacleGroup(float startX)
    {
        int groupSize = Random.Range(1, 4); // 1, 2 veya 3 obstacle

        for (int i = 0; i < groupSize; i++)
        {
            float xPos = startX + i * obstacleWidth;
            int prefabIndex = Random.Range(0, obstaclePrefabs.Length);
            Vector3 spawnPos = new Vector3(xPos, -0.25f, 0f);
            Instantiate(obstaclePrefabs[prefabIndex], spawnPos, Quaternion.identity);
        }
    }
}
