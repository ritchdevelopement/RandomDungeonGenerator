using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator {
    private GameObject allRoomsParent;
    private GameObject wallTile;
    private Dictionary<Vector2Int, Room> rooms;
    private List<Room> createdRooms = new List<Room>();
    private HashSet<Vector2Int> reservedPositions = new HashSet<Vector2Int>();
    private int numberOfRooms;
    private Vector2Int roomSize;

    public RoomGenerator(GameObject wallTile, int numberOfRooms, Vector2Int roomSize) {
        this.wallTile = wallTile ?? throw new MissingReferenceException("WallTile must not be null.");
        this.numberOfRooms = numberOfRooms;
        this.roomSize = roomSize;
        allRoomsParent = new GameObject("Rooms");
    }

    public void Run() {
        DrawRooms();
    }

    private void DrawRooms() {
        CreateRooms();

        foreach(Room room in createdRooms) {
            DrawRoom(room);
        }

        Debug.Log($"Generated {createdRooms.Count} rooms out of requested {numberOfRooms}.");
    }

    private void DrawRoom(Room room) {
        GameObject roomGameObject = new GameObject("Room_" + room.roomPos.x + "_" + room.roomPos.y);
        roomGameObject.transform.parent = allRoomsParent.transform;
        DrawWalls(room, roomGameObject);
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

    private void DrawWalls(Room room, GameObject roomGameObject) {
        GameObject wallGameObject = new GameObject("Wall");

        float left = -room.roomSize.x / 2f + room.roomPos.x;
        float right = room.roomSize.x / 2f + room.roomPos.x;
        float top = room.roomSize.y / 2f + room.roomPos.y;
        float bottom = -room.roomSize.y / 2f + room.roomPos.y;

        for(int i = 0; i <= room.roomSize.x; i++) {
            for(int j = 0; j <= room.roomSize.y; j++) {
                Vector2 tilePos = new Vector2(left + i, top - j);
                Vector2Int tile = Vector2Int.RoundToInt(tilePos);

                bool isEdgeTile =
                    Mathf.Approximately(tilePos.x, left) ||
                    Mathf.Approximately(tilePos.x, right) ||
                    Mathf.Approximately(tilePos.y, top) ||
                    Mathf.Approximately(tilePos.y, bottom);

                if(isEdgeTile && !IsDoorTile(room, tile)) {
                    Object.Instantiate(wallTile, tilePos, Quaternion.identity).transform.parent = wallGameObject.transform;
                }
            }
        }

        wallGameObject.transform.parent = roomGameObject.transform;
    }

    private bool IsDoorTile(Room room, Vector2Int tile) {
        int x = room.roomPos.x;
        int y = room.roomPos.y;
        int halfWidth = room.roomSize.x / 2;
        int halfHeight = room.roomSize.y / 2;

        var doorOffsets = new Dictionary<Direction, (bool isOpen, Vector2Int center)> {
            { Direction.North, (room.doorTop, new Vector2Int(x, y + halfHeight)) },
            { Direction.South, (room.doorBottom, new Vector2Int(x, y - halfHeight)) },
            { Direction.West, (room.doorLeft, new Vector2Int(x - halfWidth, y)) },
            { Direction.East, (room.doorRight, new Vector2Int(x + halfWidth, y)) }
        };

        foreach((Direction dir, (bool isOpen, Vector2Int center)) in doorOffsets) {
            if(!isOpen) {
                continue;
            }

            // Check if the tile is within the 3x3 area around the door center
            for(int dx = -1; dx <= 1; dx++) {
                for(int dy = -1; dy <= 1; dy++) {
                    if(tile == center + new Vector2Int(dx, dy)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
