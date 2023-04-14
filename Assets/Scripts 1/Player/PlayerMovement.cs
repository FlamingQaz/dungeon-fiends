using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;


    void Awake()
    {
        //playerAnimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //playerAnimation.movementAnim();
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;

        rb.velocity = movement;
// transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed);
    }
    

   
}
