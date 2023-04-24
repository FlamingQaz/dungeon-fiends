using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public float speed;
	[HideInInspector] public Vector2 direction;
	public float maxRange;
	[HideInInspector] public float damage;
    [HideInInspector] public LayerMask targets;
    [HideInInspector] public LayerMask friendlies;

    public void Init(LayerMask targetLayers, LayerMask friendlyLayers, float dmg, Vector2 destination) {
        direction = (destination - (Vector2)transform.position).normalized;

        targets = targetLayers;
        friendlies = friendlyLayers;
        damage = dmg;
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

        if (TargetFollower.IsTargetLayer(friendlies, collider.gameObject.layer) || 
            TargetFollower.IsTargetLayer(LayerMask.GetMask("Item"), collider.gameObject.layer) ||
            TargetFollower.IsTargetLayer(LayerMask.GetMask("Interaction Field"), collider.gameObject.layer)) 
                    return;

		if (TargetFollower.IsTargetLayer(targets, collider.gameObject.layer)) {
            Debug.Log("Is Target");
		    Entity ent = collider.gameObject.GetComponent<Entity>();
            if (ent) ent.TakeDamage(damage);
		}
        Destroy(this.gameObject);
	}

    public virtual void Shoot(Transform start, Vector2 end, LayerMask targetLayer, LayerMask friendlyLayer, float damage) {
        Instantiate(gameObject, start.position, start.rotation).GetComponent<BasicProjectile>().Init(targetLayer, friendlyLayer, damage, end);
    }
}
