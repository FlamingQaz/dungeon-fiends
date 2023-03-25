using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{

    public bool pathfind = true;
    AIPath pathfinder;

    // TODO: add enemy type enum with melee and ranged that modify slowing and stopping distance
    // TODO: add method that sets pathfinding target

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pathfind != pathfinder.canMove) pathfinder.canMove = pathfind;
    }
}
