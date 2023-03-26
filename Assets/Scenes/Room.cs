using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //Size, center position, and shape of rooms
    public Vector2Int size;
    public Vector2 centerPos;
    public Shape shape;


    //List of types of rooms
    public enum Shape
    {
        Rectangle,
        Circle
    }

    public Room(Vector2Int size, Vector2 centerPos, Shape shape)
    {
        this.size = size;
        this.centerPos = centerPos;
        this.shape = shape;
    }
}
