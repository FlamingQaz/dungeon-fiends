using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Player : MonoBehaviour
{
    [HideInInspector] public Entity entity;
}
