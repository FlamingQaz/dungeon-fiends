using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SwordData;

public class playerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnim;

    // Start is called before the first frame update
    void Awake()
    {
        playerAnim = GetComponent<Animator>();
    }

    public void movementAnim()
    {
        playerAnim.SetBool("Moving", Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0);

        Vector2 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        
        if (mouse.x < playerScreenPoint.x) 
            GetComponent<SpriteRenderer>().flipX = true;
        else 
            GetComponent<SpriteRenderer>().flipX = false;
    }

    public void playDeathAnim()
    {
        playerAnim.SetTrigger("Death");
    }
}
