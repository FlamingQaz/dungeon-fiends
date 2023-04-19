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
    public float shootingRange = 10f;
    bool isShooting = false;
    public BasicProjectile baseProjectile;
    public LayerMask targetLayer;
    public LayerMask friendlyLayer;

    protected const float STOPPING_OFFSET = -0.3f;
    protected const float SLOWING_OFFSET = 3f;

    void Start() {
        pathfinding = GetComponent<TargetFollower>();
        entity = GetComponent<Entity>();

        if (pathfinding.slowingDistance == 0f) pathfinding.slowingDistance = ((float) type + SLOWING_OFFSET);
        if (pathfinding.stoppingDistance == 0f) pathfinding.stoppingDistance = ((float) type + STOPPING_OFFSET);

        if (type == EnemyType.Ranged && !baseProjectile) Debug.LogError("A ranged enemy does not have a projectile attached.");
        pathfinding.SetTargets(targetLayer);

        entity.onEndAttack.AddListener((Entity target) => {
            if (!target.isAlive && !target.onKillTriggered) {
                entity.onKill.Invoke(target);
                target.onKillTriggered = true;
            }
        });
    }

    void FixedUpdate() {
        // Handle movement speed
        if (pathfinding.speed != entity.GetMoveSpeed()) pathfinding.speed = entity.GetMoveSpeed();

        // Handle ranged attacks
        if (type == EnemyType.Ranged) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, shootingRange, Vector2.zero, 0f, targetLayer);
            if (hit && !isShooting && pathfinding.targetInSight && hit.collider.GetComponent<Entity>().isAlive) ShootAt(hit.collider.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Handle melee attacks
        if (type != EnemyType.Melee && type != EnemyType.LongMelee) return;
        if (!TargetFollower.IsTargetLayer(targetLayer, other.gameObject.layer)) return;
        MeleeAttack(other.gameObject);
    }

    public virtual void MeleeAttack(GameObject entityObj) {
        Entity otherEntity = entityObj.GetComponent<Entity>();
        if (!otherEntity || !otherEntity.targetable) return;

        entity.onStartAttack.Invoke(otherEntity);
        otherEntity.TakeDamage(entity.GetAttackDamage(), Entity.DamageType.Combat);
        entity.onEndAttack.Invoke(otherEntity);
    }

    public virtual void ShootAt(GameObject entityObj) {
        Entity otherEntity = entityObj.GetComponent<Entity>();
        if (!otherEntity || !otherEntity.targetable) return;

        entity.onStartAttack.Invoke(otherEntity);
        baseProjectile.Shoot(transform, entityObj.transform.position, targetLayer, friendlyLayer, entity.GetAttackDamage(), entity);

        isShooting = true;
        float shootRate = entity.GetAttackSpeed();
        Invoke(nameof(EndShootCooldown), 1/shootRate);
    }

    protected void EndShootCooldown() {
        isShooting = false;
    }

}
