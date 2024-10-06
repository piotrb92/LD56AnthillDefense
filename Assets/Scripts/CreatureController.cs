using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{

    public float movementSpeed = 3, rotSpeed = 20;
    public float fogRevealRadius = 3;
    public Vector2 targetPosition;
    public float stoppingDistance;

    public Rigidbody2D rb;
    private Animator anim;
    private Creature creature;

    public float startingSpeed;

    private void Awake()
    {
        creature = GetComponent<Creature>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    public virtual void Start()
    {
        if (creature.isControlledByPlayer)
            targetPosition = transform.position;
        else
            targetPosition = GameManager.instance.anthill.transform.position;
        startingSpeed = movementSpeed;
    }

    public virtual void FixedUpdate()
    {
        if (creature.isDead) return;
        HandleMovement();
    }

    public virtual void HandleMovement()
    {
        float dist = Vector2.Distance(transform.position, targetPosition);
        if (dist <= stoppingDistance)
        {
            anim.SetFloat("Speed", 0);
            return;
        }

        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle - 90, rotSpeed* Time.deltaTime);
            rb.MovePosition(rb.position + dir * movementSpeed * Time.deltaTime);
            anim.SetFloat("Speed", 1);
            if (creature.isControlledByPlayer)
            {
                GameManager.instance.fog.RevealTilesAroundUnit(transform.position, fogRevealRadius);
            }
            rb.velocity = Vector2.zero;
        }
    }
}
