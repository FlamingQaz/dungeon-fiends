using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Entity), typeof(PlayerMovement), typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerCombat), typeof(PlayerCamera))]
public class Player : MonoBehaviour
{
    [HideInInspector] public Entity entity;
    [SerializeField] Slider healthBar;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerAnimation anim;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public PlayerInventory inventory;

    void Awake() {
        entity = GetComponent<Entity>();
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<PlayerAnimation>();
        combat = GetComponent<PlayerCombat>();

        entity.destroyOnDeath = false;
    }

    public void UpdateHealthBar() {
        healthBar.value = entity.GetHealth() / entity.GetMaxHealth();
    }
}
