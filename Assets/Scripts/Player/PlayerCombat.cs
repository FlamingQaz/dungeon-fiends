using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    [SerializeField] BasicProjectile baseProjectile;
    [SerializeField] float damage = 2f;
    Entity entity;
    
    void Start() {
        entity = GetComponent<Entity>();
    }

    void Update() {
        // Example combat for testing purposes
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            baseProjectile.Shoot(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition), enemyLayer, friendlyLayer, damage, entity);
        }
    }
}
