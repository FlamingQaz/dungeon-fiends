using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableWeaponScript : InteractableItem
{
    public GameObject weapon;
    
    public override void Interact()
    {
        FindObjectOfType<PlayerInventory>().PickUpWeapon(weapon);
        Destroy(this.gameObject);
    }
}
