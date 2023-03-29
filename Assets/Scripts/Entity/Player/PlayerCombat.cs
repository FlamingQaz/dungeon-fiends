using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    [SerializeField] BasicProjectile baseProjectile;
    [SerializeField] float damage = 2f;
    [SerializeField] float fireRate = 10f;
    Entity entity;
    bool onCooldown = false;
    
    void Start() {
        entity = GetComponent<Entity>();
        entity.onEndAttack.AddListener((Entity target) => {
            if (!target.isAlive) entity.onKill.Invoke(target);
        });
    }

    void FixedUpdate() {
        // Example combat for testing purposes
        if (Input.GetKey(KeyCode.Mouse0) && !onCooldown) {
            baseProjectile.Shoot(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition), enemyLayer, friendlyLayer, damage, entity);
            onCooldown = true;
            Invoke(nameof(EndCooldown), 1f/fireRate);
        }
    }

    // For example combat only
    void EndCooldown() {
        onCooldown = false;
    }
}
