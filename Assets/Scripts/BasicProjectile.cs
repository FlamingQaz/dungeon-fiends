using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicProjectile : MonoBehaviour
{
    public float speed;
	[HideInInspector] public Vector2 direction;
	public float maxRange;
	[HideInInspector] public float damage;
    [HideInInspector] public LayerMask targets;
    [HideInInspector] public LayerMask friendlies;
    public EntityEvent onHit = new EntityEvent();

    public BasicProjectile Init(LayerMask targetLayers, LayerMask friendlyLayers, float dmg, Vector2 destination) {
        direction = (destination - (Vector2)transform.position).normalized;

        targets = targetLayers;
        friendlies = friendlyLayers;
        damage = dmg;

        return this;
    }

	// Update is called once per frame
	void Update () {
        if (direction == Vector2.zero) Destroy(this.gameObject);
        else transform.position = (Vector2)transform.position + direction * speed * Time.deltaTime;
	}

    void FixedUpdate() {
        if (!Physics2D.CircleCast(transform.position, maxRange, Vector2.zero, 0f, targets)) Destroy(gameObject);
    }

	protected virtual void OnTriggerEnter2D(Collider2D collider) {
        if (TargetFollower.IsTargetLayer(friendlies, collider.gameObject.layer)) return;

		if (TargetFollower.IsTargetLayer(targets, collider.gameObject.layer)) {
		    Entity ent = collider.gameObject.GetComponent<Entity>();
            if (ent) {
                ent.TakeDamage(damage);
                onHit.Invoke(ent);
            }
		}

        Destroy(this.gameObject);
	}

    public virtual void Shoot(Transform start, Vector2 end, LayerMask targetLayer, LayerMask friendlyLayer, float damage, Entity shooter) {
        GameObject projectile = Instantiate(gameObject, start.position, start.rotation);

        projectile.name = gameObject.name + " (" + shooter.gameObject.name + ")";
        projectile.transform.parent = shooter.transform;
        projectile.GetComponent<BasicProjectile>()
        .Init(targetLayer, friendlyLayer, damage, end)
        .onHit.AddListener((Entity target) => shooter.onEndAttack.Invoke(target));;
    }
}
