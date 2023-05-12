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


    //How close room's center positions are together
    //- .5 is the furthest away, 0 is the closest
    public float roomSquish = .49f;

    //How likely a room will be opposite to the last generated room
    // 0-100 - 0 is completely random, 100 is all in one direction
    public int straigntnessFactor = 20;

    //The size of the optional branch lengths
    public int minBranchLength = 1;
    public int maxBranchLength = 4;
    public int minBranches = 2;
    public int maxBranches = 4;

    //Room chance List
    //Corresponds to Rectangle, Circle, Hall, Ballroom, L
    public List<int> roomChanceList = new List<int>() { 60, 10, 10, 10, 10 };

    private List<Room> rooms = new List<Room>();


    [SerializeField]
    public List<Room> path = new List<Room>();
    public TilePlacement tilePlacement;

    int[,] map;

    void Start()
    {
        GenerateLevel();


    }



    void GenerateLevel()
    {
        GenerateRooms();

        PlaceBranches();

        PlaceHalls();

        //DesignateRooms();
    }

    void GenerateRooms()
    {

        //Makes a number of Rooms, and places them down

        int numRooms = UnityEngine.Random.Range(minRooms, maxRooms);
        int rand = 0;
        Room roomPrior = new Room(new Vector2Int(0, 0), new Vector2(0, 0), Room.Shape.Rectangle);

        //Creates Rooms
        for (int i = 0; i < numRooms - 1; i++)
        {
            //
            if (UnityEngine.Random.Range(0, 100) > straigntnessFactor)
            {
                Debug.Log("Random");
                rand = (int)UnityEngine.Random.Range(0, 4);
            }

            Room room = GenerateRoom(roomPrior, rand);
            room = HandleOverlap(room);
            PlaceRoom(room);


            rooms.Add(room);
            roomPrior = room;


        }
        Debug.Log("Rooms Generated");
    }

    Room GenerateRoom(Room roomPrior, int rand)
    {

        //random shape by making random index for array of Shapes
        Room.Shape shape = FindRandomWeightedRoom();

        //random size
        int sizex = 2 * (int)(UnityEngine.Random.Range(minRoomSize.x, maxRoomSize.x) / 2);
        int sizey = 2 * (int)(UnityEngine.Random.Range(minRoomSize.y, maxRoomSize.y) / 2);



        if (shape == Room.Shape.Ballroom)
            {

            //Doubles room size
            sizex = sizex * 2;
            sizey = sizey * 2;
            shape = Room.Shape.Rectangle;
        }
            else if (shape == Room.Shape.Hall)
            {
            //Makes a room with one side longer than the other by 2 times
            int randy = UnityEngine.Random.Range(0, 10);
                if (randy >= 5)
                {
                    sizey = 2 * sizey;
                    sizex = 2 * (int)(sizex / 4);
            }
                else
                {
                    sizex = sizex * 2;
                    sizey = 2 * (int)(sizey / 4);
            }
                shape = Room.Shape.Rectangle;
            }
            else if (shape == Room.Shape.L)
            {
                sizex = sizex * 2;
                sizey = sizey * 2;

            }

        
        Vector2Int size = new Vector2Int(sizex, sizey);


        Vector2 centerPos = MakeNewRoomPosition(roomPrior, size, rand);



        // create a new room with the generated size, position, and shape, and add it to the list of rooms
        Room room = new Room(size, centerPos, shape);

        return room;
    }

    Room.Shape FindRandomWeightedRoom()
    {
        Room.Shape[] values = (Room.Shape[])Room.Shape.GetValues(typeof(Room.Shape));

        int randomIndex = UnityEngine.Random.Range(0, roomChanceList.Sum());

        for (int i = 0; i < values.Length; i++)
        {
            if (randomIndex < roomChanceList[i])
            {
                randomIndex = i;
                    break;
            }
            else
            {
                randomIndex -= roomChanceList[i];
            }
        }

        Room.Shape shape = values[randomIndex];




        return shape;
    }

    Room HandleOverlap(Room room2)
    {
        //Go through all rooms and check for overlap

        int leng = rooms.Count;
        int rand = (int)UnityEngine.Random.Range(0, 4);
        int i = 0;
        int j = 0;
        //this is here to stop infinite loops,
        //although they theoretically shouldn't happen
        while (i < leng && j < 20)
        {
            Room room1 = rooms[i];
            i++;

            if (IsOverlap(room1, room2))
            {


                room2.centerPos = MakeNewRoomPosition(room1, room2.size, rand);

                i = 0;
                j++;
                Debug.Log("Overlap Fixed");
            }


        }




        return room2;
    }

    Vector2 MakeNewRoomPosition(Room roomPrior, Vector2 size, int rand)
    {
        //random center position


        //Make a center position default at 0,0
        Vector2 centerPos = new Vector2((int)0, (int)0);
        if (rand == 1)
        {

            //Makes the center position
            // X: prior room's center position + size of both rooms / 2
            // Y: a random value attached to the prior room's walls
            //Repeats for else statements below with different x / y

            centerPos = new Vector2(roomPrior.centerPos.x +
                size.x / 2 + roomPrior.size.x / 2,
                roomPrior.centerPos.y + UnityEngine.Random.Range(-roomSquish * roomPrior.size.y, roomSquish * roomPrior.size.y) / 2);


        }
        else if (rand == 2)
        {


            centerPos = new Vector2(roomPrior.centerPos.x +
                UnityEngine.Random.Range(-roomSquish * roomPrior.size.x, roomSquish * roomPrior.size.x) / 2,
                roomPrior.centerPos.y +
                size.y / 2 + roomPrior.size.y / 2);

        }
        else if (rand == 3)
        {


            centerPos = new Vector2(roomPrior.centerPos.x -
                    size.x / 2 - roomPrior.size.x / 2,
                    roomPrior.centerPos.y + UnityEngine.Random.Range(-roomSquish * roomPrior.size.y, roomSquish * roomPrior.size.y) / 2);
        }
        else
        {


            centerPos = new Vector2(roomPrior.centerPos.x +
                UnityEngine.Random.Range(-roomSquish * roomPrior.size.x, roomSquish * roomPrior.size.x) / 2,
                roomPrior.centerPos.y
                - size.y / 2 - roomPrior.size.y / 2);
        }
        return centerPos;
    }

    bool IsOverlap(Room room1, Room room2)
    {
        //Checks if the distance between the two centers of rooms


        float xDist = Mathf.Abs(room1.size.x + room2.size.x) / 2;
        float yDist = Mathf.Abs(room1.size.y + room2.size.y) / 2;

        //We check for two since the we need room for tiling 
        if (Mathf.Abs(room1.centerPos.x - room2.centerPos.x) > xDist - 1)
        {
            return false;
        }

        else if (Mathf.Abs(room1.centerPos.y - room2.centerPos.y) > yDist - 1)
        {
            return false;
        }

        else
        {
            return true;
        }


    }

    void PlaceRectangle(Room room)
    {

        //Finds room lower left corner
        int roomx = (int)(room.centerPos.x - room.size.x / 2);
        int roomy = (int)(room.centerPos.y - room.size.y / 2);
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

    void PlaceRoom(Room room)
    {
        if (room.shape == Room.Shape.Rectangle)
        { PlaceRectangle(room); }

        else if (room.shape == Room.Shape.Circle)
        { PlaceOval(room); }

        else if (room.shape == Room.Shape.L)
        { PlaceL(room); }

        else
        {
            Debug.Log("Room does not exist");
        }
    }

    void PlaceOval(Room room)
    {

        //Finds room lower left corner
        int roomx = (int)(room.centerPos.x - room.size.x / 2);
        int roomy = (int)(room.centerPos.y - room.size.y / 2);
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
                else
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0), 0);
                }

                if (IsOval(i, j,
                    (int)(room.centerPos.x), (int)(room.centerPos.y),

                    (int)(room.size.x / 2 - 1), (int)(room.size.y / 2))
                   )
                {

                    tilePlacement.PlaceCeiling(new Vector3Int((int)i, (int)j, 0), 1);



                }

            }
        }
    }

    void PlaceL(Room room)
    {
        int rand = (int)UnityEngine.Random.Range(0, 4);

        //Finds room lower left corner
        int roomx = (int)(room.centerPos.x - room.size.x / 2);
        int roomy = (int)(room.centerPos.y - room.size.y / 2);
        


        //Sets the Floor
        //For all X in room
        for (int i = roomx; i < roomx + (int)room.size.x; i++)
        {
            //for all Y in room
            for (int j = roomy; j < roomy + (int)room.size.y; j++)
            {

                //Place tiles

                tilePlacement.PlaceFloor(new Vector3Int((int)i, (int)j, 0), 1);

                //If side walls
                if (i == roomx || i == roomx + (int)room.size.x - 1)
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }
                //If top walls
                else if (j == roomy || j == roomy + (int)room.size.y - 1)
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }

                //L Walls
                else if (rand == 0 && (i < roomx +(int)room.size.x/2-1 && j < roomy + (int)room.size.y/2 - 1))
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }
                else if (rand == 1 && (i > roomx +(int)room.size.x / 2 - 1 && j < roomy + (int)room.size.y / 2 - 1))
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }
                else if (rand == 2 && (i < roomx +(int)room.size.x / 2 - 1 && j > roomy + (int)room.size.y / 2 - 1))
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));

                }
                else if (rand == 3 && (i > roomx + (int)room.size.x / 2 - 1 && j > roomy + (int)room.size.y / 2 - 1))
                {
                    tilePlacement.PlaceSide(new Vector3Int((int)i, (int)j, 0));
                }

            }

        }
    }

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

    void PlaceHall(Room room1, Room room2)
    {
        if
        (AreRoomsTouchingInXDirection(room1, room2))
        {

            //For k is the lower room position
            //k is less than the larger room position
            //k increases by 1

            int roomMinX = (int)Mathf.Min(room2.centerPos.x, room1.centerPos.x);
            int roomMinY = (int)Mathf.Min(room2.centerPos.y, room1.centerPos.y);
            int sizeMinX = (int)Mathf.Min(room2.size.x, room1.size.x);
            int roomMaxY = (int)Mathf.Max(room2.centerPos.y, room1.centerPos.y);
            for (int k = roomMinY;
                k < roomMaxY;
                k++)
            {
                tilePlacement.PlaceFloor(new Vector3Int((int)(roomMinX + sizeMinX / 4), (int)k, 0), 1);
                if (room2.centerPos.x - 2 > (room1.centerPos.x
                - room2.size.x / 2 - room1.size.x / 2))
                {
                    tilePlacement.PlaceFloor(new Vector3Int((int)(roomMinX + sizeMinX / 4) + 1, (int)k, 0), 1);
                }
                else
                {
                    tilePlacement.PlaceFloor(new Vector3Int((int)(roomMinX + sizeMinX / 4) - 1, (int)k, 0), 1);
                }

            }

        }
        else if
        (AreRoomsTouchingInYDirection(room1, room2))
        {

            //For k is the lower room position
            //k is less than the larger room position
            //k increases by 1

            int roomMinX = (int)Mathf.Min(room2.centerPos.x, room1.centerPos.x);
            int roomMinY = (int)Mathf.Min(room2.centerPos.y, room1.centerPos.y);
            int sizeMinY = (int)Mathf.Min(room2.size.y, room1.size.y);
            int roomMaxX = (int)Mathf.Max(room2.centerPos.x, room1.centerPos.x);
            for (int k = roomMinX;
                k < roomMaxX;
                k++)
            {
                tilePlacement.PlaceFloor(new Vector3Int((int)k, (int)(roomMinY + sizeMinY / 4), 0), 1);
                if (room2.centerPos.y - 2 > (room1.centerPos.y
                - room2.size.y / 2 - room1.size.y / 2))
                {
                    tilePlacement.PlaceFloor(new Vector3Int((int)k, (int)(roomMinY + sizeMinY / 4) + 1, 0), 1);
                }
                else
                {
                    tilePlacement.PlaceFloor(new Vector3Int((int)k, (int)(roomMinY + sizeMinY / 4) - 1, 0), 1);
                }

            }

        }


    }

    void PlaceHalls()
    {
        int leng = rooms.Count;

        for (int i = 0; i < leng; i++)
        {
            for (int j = i + 1; j < leng; j++)
            {
                PlaceHall(rooms[i], rooms[j]);
            }
        }
    }

    bool AreRoomsTouchingInYDirection(Room room1, Room room2)
    {
        if ((Mathf.Abs(room1.centerPos.x - room2.centerPos.x) <= room1.size.x / 2 + room2.size.x / 2 + 1
            &&
            (Mathf.Abs(room1.centerPos.y - room2.centerPos.y) <= Mathf.Max(room1.size.y / 2 + 1, room2.size.y / 2 + 1))))
        {
            // rooms are not touching in the x direction
            return true;
        }
        else
        {
            // rooms are touching in the x direction
            return false;
        }
    }

    bool AreRoomsTouchingInXDirection(Room room1, Room room2)
    {
        if ((Mathf.Abs(room1.centerPos.y - room2.centerPos.y) <= room1.size.y / 2 + room2.size.y / 2 + 1
            &&
            (Mathf.Abs(room1.centerPos.x - room2.centerPos.x) <= Mathf.Max(room1.size.x / 2 + 1, room2.size.x / 2 + 1))))
        {
            // rooms are not touching in the x direction
            return true;
        }
        else
        {
            // rooms are touching in the x direction
            return false;
        }
    }

    void PlaceBranches()
    {
        int branches = UnityEngine.Random.Range(minBranches, maxBranches);
        int rand = (int)UnityEngine.Random.Range(0, 4);
        Room roomPrior = rooms[(int)UnityEngine.Random.Range(1, rooms.Count - 1)];
        Room room;


        for (int i = 0; i < branches; i++)
        {
            room = GenerateRoom(roomPrior, rand);
            room = HandleOverlap(room);

            PlaceRoom(room);
            PlaceHall(room, roomPrior);

            rooms.Add(room);
            roomPrior = room;
        }


        rand = (int)UnityEngine.Random.Range(0, 4);
        for (int i = 0; i<5; i++)
        {
            
            room = GenerateRoom(roomPrior, rand);
            room = HandleOverlap(room);
            PlaceRoom(room);
            rooms.Add(room);

        }

    }














}



   











