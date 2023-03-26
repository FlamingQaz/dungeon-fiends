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

    public void Init(LayerMask targetLayers, float dmg, Vector2 destination) {
        direction = (destination - (Vector2)transform.position).normalized;

        targets = targetLayers;
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

	protected virtual void OnCollisionEnter2D(Collision2D collision) {
		if (TargetFollower.IsTargetLayer(targets, collision.gameObject.layer)) {
		    Entity ent = collision.gameObject.GetComponent<Entity>();
            if (ent) ent.Hurt(damage);
		}

        Destroy(this.gameObject);
	}
}
