using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{   
    private PlayerAnimation playerAnimation;
    
    Entity entity;
    float adjustedMoveSpeed;


    void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        entity = GetComponent<Entity>();    
    }

    void FixedUpdate()
    {

        //Stops players from controlling other characters 
        if (!isOwned) {return; }

        playerAnimation.movementAnim();
        adjustedMoveSpeed = (entity.GetMoveSpeed() / 3f) * 0.1f;
        transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * adjustedMoveSpeed);
    }
}
