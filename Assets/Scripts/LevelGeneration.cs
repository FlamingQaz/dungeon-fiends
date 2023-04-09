using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;




public class LevelGeneration : MonoBehaviour
{

    public int maxRooms = 5;
    public int minRooms = 3;
    public Vector2Int maxRoomSize = new Vector2Int(20, 20);
    public Vector2Int minRoomSize = new Vector2Int(10, 10);
    private Vector2 maxPosition = new Vector2(20, 20);
    public float roomSquish = 1f;

    private List<Room> rooms = new List<Room>();


    [SerializeField]
    public List<Room> path = new List<Room>();
    public TilePlacement tilePlacement;

    int[,] map;

    void Start()
    {
        GenerateRooms();
        
        PlaceRooms();
        path = ConnectRooms(rooms);
        
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
        if (  Mathf.Abs(room1.centerPos.x - room2.centerPos.x) > xDist + 4)
        {          
            return false;     
        }
            
        else if (Mathf.Abs(room1.centerPos.y - room2.centerPos.y) > yDist + 4)
        {       
            return false;    
        }

        else { 
            return true; 
        }
        
             
    }

    public List<Room> ConnectRooms(List<Room> rooms)
    {
        // Shuffle the list of rooms to ensure randomness
        

        // Connect each room to the next one in the list
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            PlaceHall(rooms[i], rooms[i + 1]);
        }

        // Connect the last room to the first room
        PlaceHall(rooms[rooms.Count - 1], rooms[0]);

        return rooms;
    }

    void PlaceRooms()
    {
        int roomx;
        int roomy;
        //Places Room tiles
        foreach (Room room in rooms)
        {
            

            if (room.shape == Room.Shape.Rectangle)
            { PlaceRectangle(room);            } 

            else if (room.shape == Room.Shape.Circle)
            { PlaceOval(room);                }
            }Debug.Log("Rooms Placed");

    }

    void PlaceRectangle(Room room){
        int roomx;
        int roomy;
        //Finds room lower left corner
        roomx = (int)(room.centerPos.x - room.size.x / 2);
        roomy = (int)(room.centerPos.y - room.size.y / 2);
        //Sets the Floor
        //For all X in room
        for (int i = roomx; i < roomx + (int)room.size.x; i++)
        {
            //for all Y in room
            for (int j = roomy; j < roomy + (int)room.size.y; j++)
            {

                //Place tiles
                tilePlacement.PlaceFloor(new Vector3Int((int)i, (int)j, 0), 1);
                if (i == roomx || i == roomx + (int)room.size.x - 1)
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }
                if (j == roomy || j == roomy + (int)room.size.y - 1)
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }
            }

        }
    }

    void PlaceOval(Room room)
    {
        int roomx;
        int roomy;
        //Finds room lower left corner
        roomx = (int)(room.centerPos.x - room.size.x / 2);
        roomy = (int)(room.centerPos.y - room.size.y / 2);
        //Sets the Floor
        //For all X in room
        for (int i = roomx; i < roomx + (int)room.size.x; i++)
        {

            //for all Y in room
            for (int j = roomy; j < roomy + (int)room.size.y; j++)
            {

                if (IsWithinOval(i, j,
                    (int)(room.centerPos.x), (int)(room.centerPos.y),

                    (int)(room.size.x / 2 - 1), (int)(room.size.y / 2))
                   )
                    //Place tiles
                    tilePlacement.PlaceFloor(new Vector3Int((int)i, (int)j, 0), 1);

                if (IsOval(i, j,
                    (int)(room.centerPos.x), (int)(room.centerPos.y),

                    (int)(room.size.x / 2 - 1), (int)(room.size.y / 2))
                   )
                {

                    tilePlacement.PlaceCeiling(new Vector3Int((int)i, (int)j, 0), 1);



                }

            } } } 
        
    private static bool IsWithinOval(int x, int y, int cx, int cy, int rx, int ry)
    {

        float dx = (float)(x - cx) / rx;
        float dy = (float)(y - cy) / ry;
        return dx * dx + dy * dy <= 1;
    }
   
    private static bool IsOval(int x, int y, int cx, int cy, int rx, int ry)
    {

        float dx = (float)(x - cx) / rx;
        float dy = (float)(y - cy) / ry;
        return (0.7 < dx * dx + dy * dy) && (dx * dx + dy * dy <= 1);
    }
   
    void PlaceHall(Room roomBefore, Room room)
    {

        
        int xmin;
        int ymin;
        int xmax;
        int ymax;

        
                       if (roomBefore.centerPos.x < room.centerPos.x)
            {
                xmin = (int)roomBefore.centerPos.x;
                xmax = (int)room.centerPos.x;
            }
            else
            {
                xmax = (int)roomBefore.centerPos.x;
                xmin = (int)room.centerPos.x;
            }
            if (roomBefore.centerPos.y < room.centerPos.y)
            {
                ymin = (int)roomBefore.centerPos.y;
                    ymax = (int)room.centerPos.y;
            }
            else
            {
                ymax = (int)roomBefore.centerPos.y;
                ymin = (int)room.centerPos.y;
            }

        
        for (int i = xmin; i < xmax; i++)
            {
            tilePlacement.PlaceFloor(new Vector3Int((int)i, (int)ymin, 0));
            tilePlacement.PlaceFloor(new Vector3Int((int)i, (int)ymin+1, 0));

        }
        for (int j = ymin; j < ymax; j++)
        {

            tilePlacement.PlaceFloor(new Vector3Int((int)xmax, (int)j, 0));
            tilePlacement.PlaceFloor(new Vector3Int((int)xmax-1, (int)j, 0));
        }







    }

   

        




}



   











