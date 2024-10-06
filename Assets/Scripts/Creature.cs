using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Creature : Health
{
    public CreatureController controller;
    public float damage = 5;
    public float attackRate = 1;
    public GameObject selection;
    public bool isControlledByPlayer = true;
    public Item currentItem;
    public Item itemToPickup;
    public List<Health> targetsInRange = new List<Health>();
    public List<GameObject> traps = new List<GameObject>();
    public Image healthBar;
    public Collider2D col, trigger;
    public AudioSource source;
    public AudioClip deathClip, hitClip, pickupClip;
    private float attackTimer;

    private void Start()
    {
        controller = GetComponent<CreatureController>();
        if (isControlledByPlayer)
        {
            GameManager.instance.playersCreatures.Add(this);
            maxHealth += GameManager.instance.upgrade.healthBonus;
            health = maxHealth;
            controller.rb.mass += GameManager.instance.upgrade.weightBonus;
        }
    }

    public void Pickup(Item item)
    {
        controller.movementSpeed *= item.movementSpeedMultiplier;
        item.isPickedUp = true;
        item.pickedUpBy = this;
        currentItem = item;
        itemToPickup = null;
        source.PlayOneShot(pickupClip);
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        healthBar.fillAmount = health / maxHealth;
        healthBar.gameObject.SetActive(true);
    }

    public override void AddHealth(float hp)
    {
        base.AddHealth(hp); 
        if(health == maxHealth)
            healthBar.gameObject.SetActive(false);
    }

    public void DropFood()
    {
        if(currentItem != null)
        {
            currentItem.pickedUpBy = null;
            currentItem.isPickedUp = false;
            currentItem = null;
            itemToPickup = null;
        }
    }

    private void Update()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;
        else
        {
            Attack();
        }
        if (isControlledByPlayer)
        {
            if (currentItem == null) controller.movementSpeed = controller.startingSpeed + GameManager.instance.upgrade.speedBonus;
            else controller.movementSpeed = (controller.startingSpeed + GameManager.instance.upgrade.speedBonus) * currentItem.movementSpeedMultiplier;
        }
    }

    public void Attack()
    {
        if (isDead) return;
        if (targetsInRange.Count == 0) return;
        Health target = targetsInRange.Where(x => x.health > 0).FirstOrDefault();
        if (target == null) return;
        attackTimer = attackRate;
        source.PlayOneShot(hitClip);
        float dmg = Random.Range(damage * 0.8f, damage * 1.2f);
        if (isControlledByPlayer) dmg += GameManager.instance.upgrade.damageBonus;
       
        if(target != null)target.TakeDamage(dmg);
    }

    public void Select()
    {
        selection.SetActive(true);
    }

    public void Deselect()
    {
        if (isDead) return;
        selection.SetActive(false);
    }

    public override void Die()
    {
        if (isDead) return;
        base.Die();
        if(!isControlledByPlayer)
        {
            GameManager.instance.wave.spidersAlive--;
            GameManager.instance.kills++;
            if (GameManager.instance.wave.spidersAlive <= 0)
            {
                { GameManager.instance.wave.WaveCompleted(); }
            }
        } else
        {
            GameManager.instance.playersCreatures.Remove(this);
        }
        gameObject.layer = LayerMask.NameToLayer("Interactive");
        gameObject.tag = "Item";
        controller.rb.isKinematic = true;
        controller.rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        Destroy(trigger);
        col.isTrigger = true;
        if(currentItem != null)
        {
            DropFood();
        }
        if(healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
        GetComponent<Item>().canBepickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isControlledByPlayer)
        {
            if (other.CompareTag("Spider"))
            {
                targetsInRange.Add(other.GetComponent<Creature>());
            }
        }
        else
        {
            if (other.CompareTag("WebTrap"))
            {
                traps.Add(other.gameObject);
                controller.movementSpeed = 0.1f;
            }
            if (other.CompareTag("Ant"))
            {
                targetsInRange.Add(other.GetComponent<Creature>());
            }
            if (other.CompareTag("Building") || other.CompareTag("WebTrap"))
            {
                targetsInRange.Add(other.GetComponent<Health>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isControlledByPlayer)
        {
            if (other.CompareTag("Spider"))
            {
                Creature c = other.GetComponent<Creature>();
                if (targetsInRange.Contains(c))
                    targetsInRange.Remove(c);
            }
        }
        else
        {
            if (other.CompareTag("WebTrap"))
            {
                if (traps.Contains(other.gameObject))
                {
                    traps.Remove(other.gameObject);
                    if(traps.Count == 0)
                    {
                        controller.movementSpeed = controller.startingSpeed;
                    }
                }
            }
            if (other.CompareTag("Ant") || other.CompareTag("Building"))
            {
                Health c = other.GetComponent<Health>();
                if (targetsInRange.Contains(c))
                    targetsInRange.Remove(c);
            }
        }
    }
}
