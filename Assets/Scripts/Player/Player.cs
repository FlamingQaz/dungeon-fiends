using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Entity), typeof(PlayerMovement), typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerCombat), typeof(PlayerCamera), typeof(PlayerInventory))]
public class Player : MonoBehaviour
{
    [HideInInspector] public Entity entity;
    [SerializeField] Slider healthBar;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerAnimation anim;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public PlayerInventory inventory;

    [SerializeField] private List<InteractableItem> InteractablesList;
    [SerializeField] private int currentInteractable;

    void Awake() {
        entity = GetComponent<Entity>();
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<PlayerAnimation>();
        combat = GetComponent<PlayerCombat>();
        inventory = GetComponent<PlayerInventory>();

        entity.destroyOnDeath = false;

        InteractablesList = new List<InteractableItem>();
        currentInteractable = InteractablesList.Count;
    }

    public void addInteractable(GameObject obj)
    {
        if (obj.TryGetComponent(out InteractableItem InteractableObject))
        {
            if (InteractablesList.Count > 0)
                InteractablesList[currentInteractable].Unhighlight();

            InteractablesList.Add(InteractableObject);

            InteractablesList[currentInteractable].Highlight();
        }
    }

    public void removeInteractable(GameObject obj)
    {
        if (InteractablesList.Count > 0)
            if (obj.TryGetComponent(out InteractableItem InteractableObject))
            {
                InteractablesList[currentInteractable].Unhighlight();

                InteractablesList.Remove(InteractableObject);

               if (currentInteractable >= InteractablesList.Count)
               {
                   currentInteractable = Mathf.Max(currentInteractable - 1, 0);
               }

                if (InteractablesList.Count > 0)
                {
                    if (currentInteractable < 0)
                        currentInteractable = 0;
                    InteractablesList[currentInteractable].Highlight();
                }
            }
    }

    public void UpdateHealthBar() {
        healthBar.value = entity.GetHealth() / entity.GetMaxHealth();
    }

    private void IncrimentInteractables()
    {
        if (InteractablesList.Count > 0)
        {
            InteractablesList[currentInteractable].Unhighlight();
            currentInteractable = (currentInteractable + 1) % InteractablesList.Count;
            InteractablesList[currentInteractable].Highlight();
        }
    }

    private void DecrimentInteractables()
    {
        if (InteractablesList.Count > 0)
        {
            InteractablesList[currentInteractable].Unhighlight();
            // https://social.msdn.microsoft.com/Forums/en-US/f68c05bd-9822-4c88-b56c-ed9de837b2c7/c-modulus-operator-sucks?forum=xnagamestudioexpress: 
            // below was supposed to be (currentInteractable - 1) % InteractablesList.Count, but
            // C# does modulo weird. Purpose of this modulo fuckery is to make it loop back and forth
            // in the list when you scroll
            currentInteractable = ((currentInteractable + InteractablesList.Count) - 1) % InteractablesList.Count;
            InteractablesList[currentInteractable].Highlight();
        }
    }

    private void Update()
    {
        if (InteractablesList.Count > 0)
        { 
            if (Input.GetAxis("Flip Through Interactables") > 0)
            {
                IncrimentInteractables();
            }
            else if (Input.GetAxis("Flip Through Interactables") < 0)
            {
                DecrimentInteractables();
            }

            if (Input.GetButtonDown("Interact"))
            {
                InteractablesList[currentInteractable].Interact();
            }
        }
    }
}
