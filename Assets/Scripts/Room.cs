using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //Size, center position, and shape of rooms
    public Vector2Int size;
    public Vector2 centerPos;
    public Shape shape;
    public float area;


    //List of types of rooms
    public enum Shape
    {
        Rectangle,
        Circle,
        Hall,
        Ballroom,
        L
    }

    public Room(Vector2Int size, Vector2 centerPos, Shape shape)
    {
        this.size = size;
        this.centerPos = centerPos;
        this.shape = shape;


        //Finding area for cirlce or rectangle
        if (shape == Shape.Rectangle)
        {
            this.area = size.x * size.y;
        }
        else if (shape == Shape.Circle)
        {
            this.area = size.x * size.y * 3.14f / 4;
        }
    }

    public int Compare(Room room2)
    {
        //null exception
        if (room2 == null)
        {
            Debug.Log("Invalid Room Compare");
            return 1;
        }
            

        else
        {
            return this.area.CompareTo(room2.area);
        }
            
    }
    public bool Equal(Room room2)
    {
        if (room2 == null)

        { 
            Debug.Log("Invalid Room Compare");  
            return false; 
        }

        return (this.area.Equals(room2.area));
    }
  

}
