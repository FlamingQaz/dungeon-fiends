using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    [SerializeField] BasicProjectile baseProjectile;
    [SerializeField] float damage = 2f;

    void Update() {
        // Example combat for testing purposes
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 5f, Vector2.zero, 0f, enemyLayer);
            if (hit) baseProjectile.Shoot(transform, hit.transform, enemyLayer, friendlyLayer, damage);
        }
    }
}
