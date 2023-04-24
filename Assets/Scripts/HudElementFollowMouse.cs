using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudElementFollowMouse : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    public void Update()
    {
        transform.position = Input.mousePosition + offset;
    }

}
