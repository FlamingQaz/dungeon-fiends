using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionField : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = transform.parent.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "InteractableItem")
        {
            player.addInteractable(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "InteractableItem")
        {
            player.removeInteractable(collision.gameObject);
        }
    }
}
