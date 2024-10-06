using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Item : MonoBehaviour
{
    public int addFood = 10, addWood = 0, addStone = 0, addWeb = 0;
    public float movementSpeedMultiplier;
    public bool isPickedUp = false;
    public Creature pickedUpBy;
    public Vector3 offset;
    public bool canBepickedUp = true;

    private void Update()
    {
        if(isPickedUp)
        {
            if(pickedUpBy != null)
            {
                transform.position = pickedUpBy.transform.position + offset;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Ant") && !isPickedUp && canBepickedUp)
        {
            Creature c = col.GetComponent<Creature>();
            if(c.currentItem == null)
            {
                c.Pickup(this);
            }
        }
    }
}
