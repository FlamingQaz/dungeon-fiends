using System.Collections.Generic;
using UnityEngine;
using static Room;
using static RoomNode;
using UnityEngine.Tilemaps;




public class LevelGeneration : MonoBehaviour
{

    public int maxRooms = 50;
    public int minRooms = 30;
    public Vector2Int maxRoomSize = new Vector2Int(20, 20);
    public Vector2Int minRoomSize = new Vector2Int(10, 10);
    public Vector2 maxPosition = new Vector2(2, 2);
    public int highestX;
    public int lowestX;
    public int highestY;
    public int lowestY;
    
    private List<Room> rooms = new List<Room>();
    private List<Room> roomsUsed = new List<Room>();
    private List<Room> path = new List<Room>();
    private TileBase groundTile;
    private Tilemap DarkDungeon;
    

    int[,] map;

    void Start()
    {
        GenerateRooms();
        HandleOverlap();
        GenerateTopRooms();
        
        path = FindPath(roomsUsed);

        foreach (Room room in rooms)
        {
            if (room.centerPos.x - 1 - room.size.x / 2 < lowestX)
            {
                lowestX = (int)Mathf.Ceil(room.centerPos.x - 1 - room.size.x / 2);
            }
            if (room.centerPos.x + 1 + room.size.x / 2 > highestX)
            {
                highestX = (int)Mathf.Ceil(room.centerPos.x + 1 + room.size.x / 2);
            }
            if (room.centerPos.y - room.size.y/2 - 1 < lowestY)
            {
                lowestY = (int)Mathf.Ceil(room.centerPos.y - 1 - room.size.y / 2);
            }
            if (room.centerPos.y+1 + room.size.y / 2 > highestY)
            {
                highestY = (int)Mathf.Ceil(room.centerPos.y+1 + room.size.y / 2);
            }
        }

        map = GenerateArray(highestX - lowestX, highestY - lowestY, false);
        Renderer(map, DarkDungeon, groundTile);
          

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
                Room.Shape[] values = (Room.Shape[])Shape.GetValues(typeof(Room.Shape));
                int randomIndex = UnityEngine.Random.Range(0, values.Length);
                Room.Shape shape = values[randomIndex];

                // create a new room with the generated size, position, and shape, and add it to the list of rooms
                Room room = new Room(size, centerPos, shape);

                rooms.Add(room);
            }
        Debug.Log("Rooms Generated");
    } 

    void HandleOverlap()
    {
        //Go through all rooms and check for overlap
        foreach (Room room1 in rooms)
        {
            foreach (Room room2 in rooms)
            {
                if (room1 != room2)
                {

                    //If overlap exists, then translate the room over by the length of the room
                    if (!IsOverlap(room1, room2))
                    {
                        if (room2.centerPos.x > 0)
                        { room2.centerPos.x += room1.size.x; }
                        else
                        { room2.centerPos.x -= room1.size.x; }
                        if (room2.centerPos.y > 0)
                        { room2.centerPos.y += room1.size.y; }
                        else
                        { room2.centerPos.y -= room1.size.y; }
                    }
                }
            }
        }
        Debug.Log("Overlap Handled");
    }

    bool IsOverlap(Room room1, Room room2)
    {
        //Checks if the distance between the two centers of rooms


        float xDist = (room1.size.x + room2.size.x) / 2;
        float yDist = (room1.size.y + room2.size.y) / 2;
        
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

        Debug.Log("Nodes Made");
        return nodes;
    }
    
    public List<Room> FindPath(List<Room> mainRooms)
    {   // Define a function to find a path that goes through each main room once

        // Create the graph from the mainRooms list
        List<RoomNode> nodes = CreateGraph(mainRooms);

        // Perform depth-first search to find a path that goes through each node once
        List<Room> path = new List<Room>();
        HashSet<RoomNode> visited = new HashSet<RoomNode>();
        Stack<RoomNode> stack = new Stack<RoomNode>();
        stack.Push(nodes[0]);

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

        Debug.Log("Path Found");
        return path;
    }

    public void Renderer(int[,] map, Tilemap groundTileMap,TileBase groundTileBase)
    {
        for (int x = 0; x < highestX - lowestX; x++)
        {
            for (int y = 0; y < highestY - lowestY; y++)
            {
                if (map[x, y] == 1)
                    DarkDungeon.SetTile(new Vector3Int(x, y, 0), groundTileBase);
            }
        }
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x=0; x<width; x++)
        {
            for (int y=0; y < height; y++)
            {
                map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }







}















