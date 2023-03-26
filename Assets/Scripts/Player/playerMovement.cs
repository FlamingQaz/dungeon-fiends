using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private playerAnimation playerAnimation;
    
    [SerializeField]
    private float moveSpeed;


    void Awake()
    {
        playerAnimation = GetComponent<playerAnimation>();
    }

    void FixedUpdate()
    {
        playerAnimation.movementAnim();
        transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed);
    }
}
