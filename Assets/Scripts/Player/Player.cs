using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Entity))]
public class Player : MonoBehaviour
{
    [HideInInspector] public Entity entity;
    public LayerMask enemyLayer;

    [SerializeField] private Slider healthBar;

    void FixedUpdate() {
        // Example combat for testing purposes
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 5f, Vector2.zero, 0f, enemyLayer);
            if (hit) hit.collider.gameObject.GetComponent<Entity>().Hurt(2);
        }
    }

    public void updateHealthBar()
    {
        healthBar.value = GetComponent<Entity>().health / GetComponent<Entity>().maxHealth;
    }
}
