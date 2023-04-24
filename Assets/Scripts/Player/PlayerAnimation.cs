using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SwordData;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnim;

    // Start is called before the first frame update
    void Awake()
    {
        playerAnim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.Find("Weapons").rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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

    public void playResurrectionAnim()
    {
        playerAnim.SetTrigger("Resurrection");
    }
}