using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public List<GameObject> foodItems = new List<GameObject>();
    public int initialSpawnAmount = 50;
    public float spawnInterval = 10;
    public float minSpawnDistance = 10;
    public Transform anthill;
    public int itemsPerWave = 20;

    //spawn bounds
    public Vector2 spawnAreaMin = new Vector2(-50,-50);
    public Vector2 spawnAreaMax = new Vector2(50,50);

    private float spawnTimer;

    private void Start()
    {
        SpawnItems(initialSpawnAmount);
        spawnAreaMin *= 0.5f;
        spawnAreaMax *= 0.5f;
    }

    public void SpawnItems(int x)
    {
        for (int i = 0; i < x; i++)
        {
            SpawnFoodItem();
        }
    }

    public void SpawnFoodItem()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();

        GameObject foodItem = foodItems[Random.Range(0, foodItems.Count)];

        Instantiate(foodItem, spawnPosition, Quaternion.identity);
    }

    public Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition;
        float distance;

        do
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            spawnPosition = new Vector3(x, y, 0);

            distance = Vector3.Distance(spawnPosition, anthill.position);
        } while (distance < minSpawnDistance);
        return spawnPosition;
    }
}
