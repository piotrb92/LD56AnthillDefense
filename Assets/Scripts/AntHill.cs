using UnityEngine;
using UnityEngine.UI;


public class AntHill : Health
{
    public float fogRevealDistance = 30;
    public GameObject antPrefab;
    public Transform spawnPos;
    public float antSpawnInterval = 10;
    public float currentFood = 10;
    public int wood, stone, web;
    public float spawnTimer;
    public Image spawnTimerImage;
    public float waveHealthRegeneration = 20;
    public AudioSource source;
    public AudioClip addResourceClip;
    

    private void Start()
    {
        GameManager.instance.fog.RevealTilesAroundUnit(transform.position, fogRevealDistance);
    }

    private void Update()
    {
        if(currentFood > 0)
        {
            currentFood -= Time.deltaTime;
            spawnTimer += Time.deltaTime;
            spawnTimerImage.fillAmount = spawnTimer / antSpawnInterval;
            if(spawnTimer >= antSpawnInterval)
            {
                SpawnAnt();
            }
        }
        GameManager.instance.ui.SetStats((int)health, GameManager.instance.playersCreatures.Count, (int)currentFood, wood, stone, web);
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
    }

    public override void Die()
    {
        base.Die();
        GameManager.instance.GameOver();
    }

    public void SpawnAnt()
    {
        spawnTimer = 0;
        GameObject ant = Instantiate(antPrefab, spawnPos.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if(item.pickedUpBy != null)
            {
                item.pickedUpBy.DropFood();
            }
            currentFood += item.addFood;
            wood += item.addWood;
            stone += item.addStone;
            web += item.addWeb;
            source.PlayOneShot(addResourceClip);
            
            Destroy(other.gameObject);
        }
    }
}
