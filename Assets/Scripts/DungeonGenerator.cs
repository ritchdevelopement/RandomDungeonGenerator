﻿using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public Vector2Int roomSize;
    public int numberOfRooms;
    public GameObject wallTile;

    [SerializeField]
    private RoomGenerator roomGenerator;
    private Dictionary<Vector2Int, Room> rooms;
    private List<Room> createdRooms = new List<Room>();
    private HashSet<Vector2Int> reservedPositions = new HashSet<Vector2Int>();

    private void Start() {
        GenerateDungeon();
    }

    private void GenerateDungeon() {
        CreateRooms();
        Debug.Log($"Generated {createdRooms.Count} rooms out of requested {numberOfRooms}.");

        RoomGenerator roomGenerator = new RoomGenerator(wallTile);

        foreach(Room room in createdRooms) {
            roomGenerator.DrawRoom(room);
        }
    }

    private void CreateRooms() {
        rooms = new Dictionary<Vector2Int, Room>();
        Vector2Int initialRoomPos = Vector2Int.zero;

        Queue<Room> roomsToCreate = new();
        roomsToCreate.Enqueue(new Room(roomSize, initialRoomPos));
        reservedPositions.Add(initialRoomPos);

        while(roomsToCreate.Count > 0 && createdRooms.Count < numberOfRooms) {
            Room currentRoom = roomsToCreate.Dequeue();
            rooms[currentRoom.roomPos] = currentRoom;
            createdRooms.Add(currentRoom);
            AddNeighbour(currentRoom, roomsToCreate);
        }

        CreateDoors(createdRooms);
    }

    private void AddNeighbour(Room currentRoom, Queue<Room> roomsToCreate) {
        List<Vector2Int> neighbourPositions = currentRoom.GetNeighbourPositions();
        List<Vector2Int> availableNeighbours = new();

        foreach(Vector2Int position in neighbourPositions) {
            if(!rooms.ContainsKey(position) && !reservedPositions.Contains(position)) {
                availableNeighbours.Add(position);
            }
        }

        int numberOfNeighbors = Random.Range(1, availableNeighbours.Count + 1);

        for(int i = 0; i < numberOfNeighbors && availableNeighbours.Count > 0; i++) {
            Vector2Int chosen = availableNeighbours[Random.Range(0, availableNeighbours.Count)];
            availableNeighbours.Remove(chosen);
            roomsToCreate.Enqueue(new Room(roomSize, chosen));
            reservedPositions.Add(chosen);
        }
    }

    private void CreateDoors(List<Room> createdRooms) {
        foreach(Room room in createdRooms) {
            foreach(Vector2Int pos in room.GetNeighbourPositions()) {
                if(rooms.TryGetValue(pos, out Room neighbour)) {
                    room.Connect(neighbour);
                }
            }
        }
    }
}
