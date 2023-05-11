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
        Ranged = 5
    }
    public EnemyType type;
    public float shootingRange = 10f;
    bool isAttacking = false;
    Entity meleeTarget;
    public BasicProjectile baseProjectile;
    public LayerMask targetLayer;
    public LayerMask friendlyLayer;

    protected const float STOPPING_OFFSET = -0.3f;
    protected const float SLOWING_OFFSET = 3f;

    ///<summary>Use DifficultyScaling.SetLevel() instead to properly set enemy scaling.</summary>
    public static void SetLocalScaling(DifficultyScaling.Level prevDifficulty, DifficultyScaling.Level currDifficulty) {
        foreach (Enemy e in GameObject.FindObjectsOfType<Enemy>()) {
            e.entity.statMultipliers.Remove(DifficultyScaling.enemyStatScaling, (float) prevDifficulty);
            e.entity.statMultipliers.Add(DifficultyScaling.enemyStatScaling, (float) currDifficulty);
            e.entity.UpdateHeadHealthBar();
        }
    }

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

        // Handle enemy scaling:
        entity.statMultipliers.Add(DifficultyScaling.enemyStatScaling, (float) DifficultyScaling.GetLevel());
    }

    void FixedUpdate() {
        // Handle movement speed
        if (pathfinding.speed != entity.GetMoveSpeed()) pathfinding.speed = entity.GetMoveSpeed();

        // Handle ranged attacks
        if (type == EnemyType.Ranged) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, shootingRange, Vector2.zero, 0f, targetLayer);
            if (hit && !isAttacking && pathfinding.targetInSight && hit.collider.GetComponent<Entity>().isAlive) ShootAt(hit.collider.gameObject);
        }
        // Handle melee attacks
        else if (type == EnemyType.Melee) {
            if (!isAttacking && pathfinding.targetInSight && meleeTarget != null && meleeTarget.isAlive) MeleeAttack(meleeTarget.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Handle melee attacks
        if (type != EnemyType.Melee) return;
        if (!TargetFollower.IsTargetLayer(targetLayer, other.gameObject.layer)) return;
        pathfinding.enable = false;
        meleeTarget = other.gameObject.GetComponent<Entity>();
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (type != EnemyType.Melee) return;
        if (!TargetFollower.IsTargetLayer(targetLayer, other.gameObject.layer)) return;
        pathfinding.enable = true;
        meleeTarget = null;
    }

    public virtual void MeleeAttack(GameObject entityObj) {
        Entity otherEntity = entityObj.GetComponent<Entity>();
        if (!otherEntity || !otherEntity.targetable) return;

        entity.onStartAttack.Invoke(otherEntity);
        entity.Attack(otherEntity);
        entity.onEndAttack.Invoke(otherEntity);

        isAttacking = true;
        Invoke(nameof(EndAttackCooldown), 1/entity.GetAttackSpeed());
    }

    public virtual void ShootAt(GameObject entityObj) {
        Entity otherEntity = entityObj.GetComponent<Entity>();
        if (!otherEntity || !otherEntity.targetable) return;

        entity.onStartAttack.Invoke(otherEntity);
        baseProjectile.Shoot(transform, entityObj.transform.position, targetLayer, friendlyLayer, entity.GetAttackDamage(), entity);

        isAttacking = true;
        Invoke(nameof(EndAttackCooldown), 1/entity.GetAttackSpeed());
    }

    protected void EndAttackCooldown() {
        isAttacking = false;
    }

    ///<summary>This method creates a new Enemy from the game object of this Enemy. Only use this method if this Enemy is a living entity currently in the scene.</summary>
    public virtual GameObject Clone(Vector2? newPosition=null) {
        GameObject obj = Instantiate(gameObject, newPosition ?? transform.position, transform.rotation);

        // Prevent enemy scaling from exponentially growing for duplicated slimes:
        obj.GetComponent<Entity>().statMultipliers.Remove(DifficultyScaling.enemyStatScaling, (float) DifficultyScaling.GetLevel());

        return obj;
    }

}
