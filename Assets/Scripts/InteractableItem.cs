using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    [SerializeField] private Sprite itemGlow;

    public abstract void Interact();

    public virtual void Highlight()
    {
        // Implement Highlight Function here
        GameObject highLight = new GameObject();
        highLight.name = "Highlight";
        highLight.transform.SetParent(transform);
        highLight.transform.localPosition = new Vector2(0f, 0f);
        highLight.AddComponent<SpriteRenderer>();
        highLight.GetComponent<SpriteRenderer>().sprite = itemGlow;
        highLight.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
    }

    public virtual void Unhighlight()
    {
        Destroy(transform.Find("Highlight").gameObject);
    }
}
