using System.Collections.Generic;
using UnityEngine;
using static Room;







namespace LevelGeneration
{

    public class LevelGeneration : MonoBehaviour
    {

        public int maxRooms = 50;
        public int minRooms = 30;
        public Vector2Int maxRoomSize = new Vector2Int(20, 20);
        public Vector2Int minRoomSize = new Vector2Int(10, 10);
        public Vector2 maxPosition = new Vector2(2, 2);

        private List<Room> rooms = new List<Room>();



        public GameObject roomPrefab;
        // Create a list to hold the generated room game objects
        private List<GameObject> roomObjects = new List<GameObject>();
        bool IsOverlap(Room room1, Room room2)
        { 
            Bounds bounds1 = room1.GetComponent<Renderer>().bounds;
            Bounds bounds2 = room2.GetComponent<Renderer>().bounds;

            return bounds1.Intersects(bounds2);
        }

        void Start()
        {
            int numRooms = UnityEngine.Random.Range(minRooms, maxRooms);
            //Creates Rooms with random size, center position, and shape
            for (int i = 0; i < numRooms; i++)
            {
                //random size
                Vector2Int size = new Vector2Int(UnityEngine.Random.Range(minRoomSize.x, maxRoomSize.x), UnityEngine.Random.Range(minRoomSize.y, maxRoomSize.y));

                //random center position
                Vector2 centerPos = new Vector2(UnityEngine.Random.Range(-maxPosition.x, maxPosition.x), UnityEngine.Random.Range(-maxPosition.y, maxPosition.y));

                //random shape
                Room.Shape shape = (UnityEngine.Random.Range(0f, 1f) < 0.5f) ? Room.Shape.Rectangle : Room.Shape.Circle;


                // create a new room with the generated size, position, and shape, and add it to the list of rooms
                Room room = new Room(size, centerPos, shape);

                rooms.Add(room);
            
            }

            Room r1 = rooms[0];
            Room r2 = rooms[1];
            IsOverlap(r1,r2);
        }
    }
}













