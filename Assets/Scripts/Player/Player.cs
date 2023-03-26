using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Player : MonoBehaviour
{
    [HideInInspector] public Entity entity;
    public LayerMask enemyLayer;

    void Update() {
        // Example combat for testing purposes
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 5f, Vector2.zero, 0f, enemyLayer);
            if (hit) hit.collider.gameObject.GetComponent<Entity>().Hurt(2);
        }
    }
}
