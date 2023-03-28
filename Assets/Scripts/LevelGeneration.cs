using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;




public class LevelGeneration : MonoBehaviour
{

    public int maxRooms = 5;
    public int minRooms = 3;
    public Vector2Int maxRoomSize = new Vector2Int(20, 20);
    public Vector2Int minRoomSize = new Vector2Int(10, 10);
    private Vector2 maxPosition = new Vector2(2, 2);


    private List<Room> rooms = new List<Room>();
    [SerializeField]
    public List<Room> roomsUsed = new List<Room>();
    [SerializeField]
    public List<Room> path = new List<Room>();
    public TilePlacement tilePlacement;
    public List<Vector3Int> positions;

    int[,] map;

    void Start()
    {
        GenerateRooms();
        GenerateTopRooms();
        
        path = FindPath(roomsUsed);

        foreach (Room room in roomsUsed)
        {
            positions.Add(new Vector3Int((int)room.centerPos.x, (int)room.centerPos.y, 0));
        }


        tilePlacement.PlaceTiles(positions);

    }








    void GenerateRooms()
    {
        int numRooms = UnityEngine.Random.Range(minRooms, maxRooms);

            //Creates Rooms with random size, center position, and shape
            for (int i = 0; i<numRooms; i++)
            {
                //random size
                Vector2Int size = new Vector2Int(UnityEngine.Random.Range(minRoomSize.x, maxRoomSize.x), UnityEngine.Random.Range(minRoomSize.y, maxRoomSize.y));

                //random center position
                Vector2 centerPos = new Vector2(UnityEngine.Random.Range(-maxPosition.x, maxPosition.x), UnityEngine.Random.Range(-maxPosition.y, maxPosition.y));

                //random shape by making random index for array of Shapes
                Room.Shape[] values = (Room.Shape[])Room.Shape.GetValues(typeof(Room.Shape));
                int randomIndex = UnityEngine.Random.Range(0, values.Length);
                Room.Shape shape = values[randomIndex];

                // create a new room with the generated size, position, and shape, and add it to the list of rooms
                Room room = new Room(size, centerPos, shape);

                room = HandleOverlap(room);
                rooms.Add(room);
            }
        Debug.Log("Rooms Generated");
    } 

    Room HandleOverlap(Room room2)
    {
        //Go through all rooms and check for overlap
        int randPos;

        foreach (Room room1 in rooms)
        {

                    //If overlap exists, then translate the room over by the length of the room
                    if (IsOverlap(room1, room2))
                    {
                randPos = UnityEngine.Random.Range(0, 2);
                        if (room2.centerPos.x > 0 && randPos < 1)
                        {
                            //For all of these, this takes
                            //the min or max ( depends on if in the positive or negative)
                            //and makes a new vector, moving the center position by a distance of half of each room 
                            room2.centerPos = new Vector2(room1.size.x/2 + room2.size.x/2 +2 +
                                Mathf.Max(room2.centerPos.x,room1.centerPos.x), 
                                Mathf.Max(room2.centerPos.y,room1.centerPos.y));
                        }

                        else if (randPos < 1)
                        { 
                            room2.centerPos = new Vector2(-room1.size.x/2 - room2.size.x / 2-2+ 
                                Mathf.Min(room2.centerPos.x, room1.centerPos.x), 
                                Mathf.Min(room2.centerPos.y, room1.centerPos.y));
                        }

                        else if (room2.centerPos.y > 0)
                        { 
                            room2.centerPos = new Vector2(Mathf.Max(room2.centerPos.x, room1.centerPos.x), 
                                Mathf.Max(room2.centerPos.y, room1.centerPos.y)+ room1.size.y/2+ room2.size.y / 2+2);
                        }

                        else
                        { 
                            room2.centerPos = new Vector2(Mathf.Min(room2.centerPos.x, room1.centerPos.x), 
                                Mathf.Min(room2.centerPos.y, room1.centerPos.y) - room1.size.y/2 - room2.size.y / 2-2);
                        }
                    }
               
        }

        return room2;
    }

    bool IsOverlap(Room room1, Room room2)
    {
        //Checks if the distance between the two centers of rooms


        float xDist = Mathf.Abs(room1.size.x + room2.size.x)/2;
        float yDist = Mathf.Abs(room1.size.y + room2.size.y)/2;

        //We check for two since the we need room for tiling 
        if (  Mathf.Abs(room1.centerPos.x - room2.centerPos.x) > xDist + 2)
        {          
            return false;     
        }
            
        else if (Mathf.Abs(room1.centerPos.y - room2.centerPos.y) > yDist + 2)
        {       
            return false;    
        }

        else { 
            return true; 
        }
        
             
    }

    void GenerateTopRooms()
    {
        //Sort Rooms based on size
        rooms.Sort((a, b) => a.area.CompareTo(b.area));

        //Find number of rooms to use
        int numMainRooms = Mathf.FloorToInt(rooms.Count * 0.3f) + 1;
        
        //add top rooms to rooms to use
        for (int i = 0; i < numMainRooms; i++)
        {
            roomsUsed.Add(rooms[i]);
        }
        Debug.Log("Top Rooms Found");
    }

    public List<RoomNode> CreateGraph(List<Room> mainRooms)
    {
        // Create a RoomNode for each main room
        List<RoomNode> nodes = new List<RoomNode>();
        foreach (Room room in mainRooms)
        {
            RoomNode node = new RoomNode(room);
            nodes.Add(node);
        }

        // Connect neighboring rooms with edges in the graph
        for (int i = 0; i < nodes.Count; i++)
        {
            RoomNode nodeA = nodes[i];
            for (int j = i + 1; j < nodes.Count; j++)
            {
                RoomNode nodeB = nodes[j];
                if (IsOverlap(nodeA.room, nodeB.room))
                {
                    nodeA.neighbors.Add(nodeB);
                    nodeB.neighbors.Add(nodeA);
                }
            }
        }

        return nodes;
    }

    public List<Room> FindPath(List<Room> mainRooms)
    {
        // Define a function to find a path that goes through each main room once

        // Create the graph from the mainRooms list
        List<RoomNode> nodes = CreateGraph(mainRooms);

        // Find the two rooms near the end of the mainRooms list
        RoomNode endRoom1 = nodes[nodes.Count - 1];
        RoomNode endRoom2 = nodes[nodes.Count - 2];

            

        // Find the two rooms near the beginning of the mainRooms list
        RoomNode startRoom1 = nodes[0];
        RoomNode startRoom2 = nodes[1];

        // Perform depth-first search to find a path that goes through each node once
        List<Room> path = new List<Room>();
        HashSet<RoomNode> visited = new HashSet<RoomNode>();
        Stack<RoomNode> stack = new Stack<RoomNode>();
        stack.Push(startRoom1);

        while (stack.Count > 0)
        {
            RoomNode current = stack.Pop();
            if (!visited.Contains(current))
            {
                visited.Add(current);
                path.Add(current.room);
                foreach (RoomNode neighbor in current.neighbors)
                {
                    stack.Push(neighbor);
                }
            }
        }

        // Insert connections between the end rooms and the beginning rooms
   
            int index1 = path.IndexOf(startRoom1.room);
            int index2 = path.IndexOf(startRoom2.room);
            if (index1 > index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }
            path.Insert(index1 + 1, endRoom1.room);
            path.Insert(index2 + 1, endRoom2.room);
        


        Debug.Log("Path Found");
        return path;
    }









}















