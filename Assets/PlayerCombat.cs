using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    private Gun mainHand = new Gun();
    private Gun offHand = new Gun();

    




    public void PickUpGun(Gun gun)
    {

        Gun currentGun = mainHand;
        if (currentGun != null)
        {
            Destroy(currentGun.gameObject);
        }

        mainHand = Instantiate(gun);
    }

    
}
