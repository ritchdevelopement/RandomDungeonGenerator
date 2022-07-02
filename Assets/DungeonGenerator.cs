using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public Vector2Int roomSize;

    Room[,] rooms;

    public int numberOfRooms;

 
    private List<Room> createdRooms = new List<Room>();
    private RoomGenerator roomGenerator;

    // Start is called before the first frame update
    private void Start()
    {
        roomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();
        CreateRooms();
        GenerateDungeon();
    }

    private void CreateRooms() {
        int dungeonSize = numberOfRooms * roomSize.x * 3;
        rooms = new Room[dungeonSize, dungeonSize];

        Vector2Int initialRoomPos = new Vector2Int(dungeonSize / 2, dungeonSize / 2);

        Queue<Room> roomsToCreate = new Queue<Room>();
        roomsToCreate.Enqueue(new Room(roomSize, initialRoomPos));

        while(roomsToCreate.Count > 0 && createdRooms.Count < numberOfRooms) {
            Room currentRoom = roomsToCreate.Dequeue();
            rooms[currentRoom.roomPos.x, currentRoom.roomPos.y] = currentRoom;
            createdRooms.Add(currentRoom);
            AddNeighbour(currentRoom, roomsToCreate);
        }

        CreateDoors(createdRooms);
    }

    private void AddNeighbour(Room currentRoom, Queue<Room> roomsToCreate) {
        List<Vector2Int> neighbourPositions = currentRoom.GetNeighbourPositions();
        List<Vector2Int> availableNeighbours = new List<Vector2Int>();
        foreach(Vector2Int position in neighbourPositions) {
            if(rooms[position.x, position.y] == null) {
                availableNeighbours.Add(position);
            }
        }

        //Vector2Int randomRoomPos = availableNeighbours[Random.Range(0, availableNeighbours.Count)];

        //foreach(Room room in createdRooms) {
        //    List<Vector2Int> neighborCoordinates = room.NeighborCoordinates();
        //    foreach(Vector2Int coordinate in neighborCoordinates) {
        //        Room neighbor = this.rooms[coordinate.x, coordinate.y];
        //        if(neighbor != null) {
        //            room.Connect(neighbor);
        //        }
        //    }
        //}

        //roomsToCreate.Enqueue(new Room(roomSize, randomRoomPos));

        int numberOfNeighbors = (int)Random.Range(1, availableNeighbours.Count);

        for(int neighborIndex = 0; neighborIndex < numberOfNeighbors; neighborIndex++) {
            float randomNumber = Random.value;
            float roomFrac = 1f / (float)availableNeighbours.Count;
            Vector2Int chosenNeighbor = new Vector2Int(0, 0);
            foreach(Vector2Int coordinate in availableNeighbours) {
                if(randomNumber < roomFrac) {
                    chosenNeighbor = coordinate;
                    break;
                } else {
                    roomFrac += 1f / (float)availableNeighbours.Count;
                }
            }

            availableNeighbours.Remove(chosenNeighbor);
            roomsToCreate.Enqueue(new Room(roomSize, chosenNeighbor));
        }
    }

    private void CreateDoors(List<Room> createdRooms) {
        foreach(Room room in createdRooms) {
            List<Vector2Int> neighbourPositions = room.GetNeighbourPositions();
            foreach(Vector2Int position in neighbourPositions) {
                Room neighbour = rooms[position.x, position.y];
                if(neighbour != null) {
                    room.Connect(neighbour);
                }
            }
        }
    }


    private void GenerateDungeon() {
        foreach(Room room in createdRooms) {
            roomGenerator.DrawRoom(room);
        }
    }
}
