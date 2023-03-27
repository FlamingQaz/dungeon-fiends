using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    
    [SerializeField]
    private float moveSpeed;


    void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void FixedUpdate()
    {
        playerAnimation.movementAnim();
        transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed);
    }
}
