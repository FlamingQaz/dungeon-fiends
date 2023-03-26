using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TargetFollower), typeof(Entity))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public TargetFollower pathfinding;
    [HideInInspector] public Entity entity;

    public enum EnemyType {
        Melee = 1,
        Ranged = 5,
        LongMelee = 3
    }
    public EnemyType type;
    public float damage = 2f;
    public float shootingRange = 10f;
    public float shootRate = 2f; // Seconds
    bool isShooting = false;
    public GameObject baseProjectile;
    public LayerMask targetLayer;

    public UnityEvent onStartAttack;

    void Start() {
        pathfinding = GetComponent<TargetFollower>();

        if (pathfinding.slowingDistance == 0f) pathfinding.slowingDistance = ((float) type + 3f);
        if (pathfinding.stoppingDistance == 0f) pathfinding.stoppingDistance = ((float) type - 0.3f);

        if (type == EnemyType.Ranged && !baseProjectile) Debug.LogError("A ranged enemy does not have a projectile attached.");
        pathfinding.SetTargets(targetLayer);
    }

    void FixedUpdate() {
        // Handle ranged attacks
        if (type == EnemyType.Ranged) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, shootingRange, Vector2.zero, 0f, targetLayer);
            if (hit && !isShooting) ShootAt(hit.collider.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Handle melee attacks
        if (type != EnemyType.Melee && type != EnemyType.LongMelee) return;
        if (!TargetFollower.IsTargetLayer(targetLayer, other.gameObject.layer)) return;
        MeleeAttack(other.gameObject);
    }

    public virtual void MeleeAttack(GameObject entityObj) {
        Entity entity = entityObj.GetComponent<Entity>();
        onStartAttack.Invoke();
        entity.Hurt(damage);
    }

    public virtual void ShootAt(GameObject entityObj) {
        Entity entity = entityObj.GetComponent<Entity>();
        onStartAttack.Invoke();
        Instantiate(baseProjectile, transform.position, transform.rotation).GetComponent<BasicProjectile>().Init(targetLayer, damage, entityObj.transform.position);
        isShooting = true;
        Invoke(nameof(EndShootCooldown), shootRate);
    }

    protected void EndShootCooldown() {
        isShooting = false;
    }

}
