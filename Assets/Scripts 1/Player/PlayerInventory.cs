using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Gun mainHand;
    public Gun offHand;

    public void PickUpGun(Gun gun)
    {

        

        
        gun.GetComponent<Collider>().enabled = false;
        
    }
}
