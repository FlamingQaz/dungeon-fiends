using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(AIPath))]
[RequireComponent(typeof(Seeker), typeof(AIDestinationSetter))]
public class TargetFollower : MonoBehaviour
{

    bool canMove = true;
    public bool enable = true;
    AIPath pathfinder;
    AIDestinationSetter dest;
    public float speed = 3f;
    public float slowingDistance = 0f;
    public float stoppingDistance = 0f;
    public float trackingRange = 15f;
    [SerializeField] LayerMask targetLayer;
    SpriteRenderer sprite;

    void Start()
    {
        pathfinder = GetComponent<AIPath>();
        dest = GetComponent<AIDestinationSetter>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable) canMove = false;
        if (canMove != pathfinder.canMove) pathfinder.canMove = canMove;
        if (speed != pathfinder.maxSpeed) pathfinder.maxSpeed = speed;
        if (slowingDistance != pathfinder.slowdownDistance) pathfinder.slowdownDistance = slowingDistance;
        if (stoppingDistance != pathfinder.endReachedDistance) pathfinder.endReachedDistance = stoppingDistance;
    }

    void FixedUpdate() {
        if (enable) DetectTargets();
    }

    public void SetTargets(LayerMask layer) {
        targetLayer = layer;   
    }

    void DetectTargets() {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, trackingRange, Vector2.zero, 0f, targetLayer);
        if (hit) {
            if (dest.target != hit.transform) dest.target = hit.transform;
            if (hit.transform.position.x - transform.position.x < 0f) sprite.flipX = true;
            else sprite.flipX = false;

            canMove = true;
        }
        else canMove = false;
    }

    public bool IsMoving() {
        return enable && canMove;
    }

    public static bool IsTargetLayer(LayerMask targets, int layer) {
        return (targets & (1 << layer)) != 0;
    }
}
