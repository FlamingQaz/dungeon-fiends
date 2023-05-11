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
    public LayerMask obstacleLayer;
    SpriteRenderer sprite;
    [SerializeField] bool logDebugMessages = false;
    [HideInInspector] public bool targetInSight = false;

    void Start()
    {
        pathfinder = GetComponent<AIPath>();
        dest = GetComponent<AIDestinationSetter>();
        sprite = GetComponent<SpriteRenderer>();

        pathfinder.endReachedDistance = 0f;

        AstarPath.active.logPathResults = logDebugMessages ? PathLog.Normal : PathLog.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable) canMove = false;
        if (canMove != pathfinder.canMove) pathfinder.canMove = canMove;
        if (speed != pathfinder.maxSpeed) pathfinder.maxSpeed = speed;
        if (slowingDistance != pathfinder.slowdownDistance) pathfinder.slowdownDistance = slowingDistance;
    }

    void FixedUpdate() {
        if (enable) DetectTargets();
    }

    public void SetTargets(LayerMask layer) {
        targetLayer = layer;   
    }

    void DetectTargets() {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, trackingRange, Vector2.zero, 0f, targetLayer);
        targetInSight = false;

        if (hit) {
            if (dest.target != hit.transform) dest.target = hit.transform;
            if (hit.transform.position.x - transform.position.x < 0f) sprite.flipX = true;
            else sprite.flipX = false;

            Vector2 direction = (hit.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            RaycastHit2D sightRayA = Physics2D.Raycast((Vector2)transform.position + Vector2.Perpendicular(direction) * 0.3f, direction, distance, obstacleLayer);
            RaycastHit2D sightRayB = Physics2D.Raycast((Vector2)transform.position - Vector2.Perpendicular(direction) * 0.3f, direction, distance, obstacleLayer);

            if (!sightRayA && !sightRayB) targetInSight = true;

            if (targetInSight) {
                if (Vector2.Distance(transform.position, hit.transform.position) > stoppingDistance) canMove = true;
                else canMove = false; // Manual stop if within stopping distance
            }
            else canMove = true; // Keep moving if current target in range but not in sight
        }
        else canMove = false;
    }

    public bool IsMoving() {
        return enable && canMove;
    }

    public static bool IsTargetLayer(LayerMask targets, int layer) {
        return ((targets.value & (1 << layer)) > 0);
    }
}
