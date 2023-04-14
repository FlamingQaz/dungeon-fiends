using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    //this serialize field is temp.
   

    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    

    Vector2 mousePosition;
    

    void Update() {

    if (Input.GetMouseButtonDown(0))
        {
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();
            playerInventory.mainHand.Shoot();
        }
           
    }



    private void FixedUpdate()
    {
        Vector2 aimDirection = mousePosition - rb.position;
    }





    

}

