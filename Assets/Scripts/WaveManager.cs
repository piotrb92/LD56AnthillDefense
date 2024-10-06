using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public int currentWave;
    public TMP_Text waveTimerText;
    public TMP_Text waveText;
    public TMP_Text waveNumber;
    public TMP_Text waveSpawnAmountText;
    public float timeUntilWave = 60;
    public int currentWaveSpawnAmount = 1;
    public Transform[] spawnPositions;
    public bool waveInProgress = false;
    public int spidersAlive;
    public float timeBetweenSpawns = 5;
    public float maxSpiderSizeBoost = 0;

    public GameObject spiderPrefabs;

    private float waveTimer;

    private void Start()
    {
        waveTimer = timeUntilWave;
    }


    private void Update()
    {
        if(!waveInProgress)
        {
            waveTimer -= Time.deltaTime;
            waveTimerText.text = ((int)waveTimer).ToString();
            if(waveTimer <= 0)
            {
                StartCoroutine(SpawnWave());
            }
        }
        
    }

    public IEnumerator SpawnWave()
    {
        if (waveInProgress) yield break;
        waveInProgress = true;
        waveText.text = "In";
        waveTimerText.text = "Progress";
        for (int i = 0; i < currentWaveSpawnAmount; i++)
        {
            Vector3 pos = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
            GameObject spider = Instantiate(spiderPrefabs, pos, Quaternion.identity);
            float sizeBoost = Random.Range(0, maxSpiderSizeBoost);
            Creature c = spider.GetComponent<Creature>();
            c.maxHealth *= 1 + sizeBoost;
            c.health *= 1 + sizeBoost;
            c.damage *= 1 + sizeBoost;
            spider.transform.localScale *= 1 + sizeBoost;
            c.controller.movementSpeed *= 1 + sizeBoost;
            spidersAlive++;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        GameManager.instance.spawner.SpawnItems(GameManager.instance.spawner.itemsPerWave);
    }

    public void WaveCompleted()
    {
        currentWave++;
        maxSpiderSizeBoost += spiderPrefabs.transform.localScale.x * 0.1f;
        waveNumber.text = currentWave.ToString();
        waveTimer = timeUntilWave;
        currentWaveSpawnAmount = currentWave + currentWave/4;
        waveSpawnAmountText.text = currentWaveSpawnAmount.ToString();
        waveText.text = "Starts In";
        waveInProgress = false;
        GameManager.instance.anthill.AddHealth(GameManager.instance.anthill.waveHealthRegeneration);
        for (int i = 0; i < GameManager.instance.playersCreatures.Count; i++)
        {
            GameManager.instance.playersCreatures[i].AddHealth(GameManager.instance.playersCreatures[i].maxHealth);
        }
    }
}
