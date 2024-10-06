using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float health = 100, maxHealth = 100;
    public GameObject deathEffect;

    public UnityEvent OnDeath;
    public bool isDead = false;
    public bool destroyOnDeath = false;
    public virtual void TakeDamage(float dmg)
    {
        if (isDead) return;
        health -= dmg;
        if(health <= 0)
        {
            Die();
        }
    }

    public virtual void AddHealth(float hp)
    {
        health += hp;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public virtual void Die()
    {
        if (isDead) return;
        if(deathEffect != null)
        {
            GameObject deathEffectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(deathEffectInstance, 60);
        }
        isDead = true;
        OnDeath.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
    }
}
