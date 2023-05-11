using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode 
{
    public Room room;
    public List<RoomNode> neighbors;

    public RoomNode(Room room)
    {
        this.room = room;
        this.neighbors = new List<RoomNode>();
    }
}
