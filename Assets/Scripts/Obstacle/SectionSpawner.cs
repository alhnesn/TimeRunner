using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SectionSpawner : MonoBehaviour
{
    public GameObject[] sectionPrefabs;
    public Transform player;
    public float spawnDistanceAhead = 30f;
    public float deleteDistanceBehind = 40f;

    private float nextSpawnX = 0f;
    private List<GameObject> spawnedSections = new List<GameObject>();

    void Start()
    {
        nextSpawnX = player.position.x + spawnDistanceAhead;
    }

    void Update()
    {
        // Spawn new section if needed
        if (player.position.x + spawnDistanceAhead >= nextSpawnX)
        {
            GameObject section = SpawnRandomSection();
            float width = GetSectionWidth(section);
            nextSpawnX += width;
            spawnedSections.Add(section);
        }

        // Delete passed sections
        for (int i = spawnedSections.Count - 1; i >= 0; i--)
        {
            if (player.position.x - spawnedSections[i].transform.position.x > deleteDistanceBehind)
            {
                Destroy(spawnedSections[i]);
                spawnedSections.RemoveAt(i);
            }
        }
    }

    GameObject SpawnRandomSection()
    {
        int index = Random.Range(0, sectionPrefabs.Length);
        Vector3 spawnPosition = new Vector3(nextSpawnX, 0f, 0f); // Y = 0 ensures it's visible
        GameObject sectionInstance = Instantiate(sectionPrefabs[index], spawnPosition, Quaternion.identity);
        sectionInstance.SetActive(true); // Make sure it's active
        return sectionInstance;
    }

    float GetSectionWidth(GameObject section)
    {
        Tilemap tilemap = section.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            Bounds bounds = tilemap.localBounds;
            return bounds.size.x;
        }
        else
        {
            Debug.LogWarning("No Tilemap found in section!");
            return 20f; // Fallback
        }
    }
}
