using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : GeneratorBase {
    private HashSet<Vector2Int> reservedPositions = new();

    private Tilemap tilemap;
    private string tilemapName = "WallsTilemap";

    public override void Run() {
        GameObject gridGO = new GameObject("Dungeon");
        Grid grid = gridGO.AddComponent<Grid>();
        grid.cellSize = new Vector3Int(1, 1, 0);

        GameObject tilemapGO = new GameObject(tilemapName);
        tilemapGO.transform.parent = gridGO.transform;

        tilemap = tilemapGO.AddComponent<Tilemap>();
        TilemapRenderer renderer = tilemapGO.AddComponent<TilemapRenderer>();
        renderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;

        if(context.wallTile == null) {
            throw new MissingReferenceException($"No wall tile set for rooms: {context.wallTile}");
        }

        reservedPositions.Clear();

        CreateRooms();
        DrawRooms();

        Debug.Log($"Generated {context.createdRooms.Count} rooms out of requested {context.numberOfRooms}.");
    }

    private void DrawRooms() {
        foreach(Room room in context.createdRooms.Values) {
            DrawWalls(room);
        }
    }

    private void CreateRooms() {
        Vector2Int initialRoomPos = Vector2Int.zero;

        Queue<Room> roomsToCreate = new();
        roomsToCreate.Enqueue(new Room(context.roomSize, initialRoomPos));
        reservedPositions.Add(initialRoomPos);

        while(roomsToCreate.Count > 0 && context.createdRooms.Count < context.numberOfRooms) {
            Room currentRoom = roomsToCreate.Dequeue();
            context.createdRooms[currentRoom.RoomPos] = currentRoom;
            AddNeighbour(currentRoom, roomsToCreate);
        }

        CreateDoors();
    }

    private void AddNeighbour(Room currentRoom, Queue<Room> roomsToCreate) {
        List<Vector2Int> neighbourPositions = currentRoom.GetNeighbourPositions();
        List<Vector2Int> availableNeighbours = new();

        foreach(Vector2Int position in neighbourPositions) {
            if(context.createdRooms.ContainsKey(position) && reservedPositions.Contains(position)) {
                continue;
            }

            availableNeighbours.Add(position);
        }

        if(context.createdRooms.Count > context.roomDistributionFactor) {
            RemoveClosestRoomToCenter(availableNeighbours);
        }

        int maxNumberOfNeighbors = Random.Range(1, availableNeighbours.Count + 1);

        for(int i = 0; i < maxNumberOfNeighbors && availableNeighbours.Count > 0; i++) {
            Vector2Int chosen = availableNeighbours[Random.Range(0, availableNeighbours.Count)];
            availableNeighbours.Remove(chosen);
            roomsToCreate.Enqueue(new Room(context.roomSize, chosen));
            reservedPositions.Add(chosen);
        }
    }

    private void RemoveClosestRoomToCenter(List<Vector2Int> availableNeighbours) {
        if(availableNeighbours.Count == 0) {
            return;
        }

        Vector2Int closest = availableNeighbours
            .OrderBy(pos => Vector2Int.Distance(pos, Vector2Int.zero))
            .First();

        availableNeighbours.Remove(closest);
    }

    private void CreateDoors() {
        foreach(Room room in context.createdRooms.Values) {
            foreach(Vector2Int pos in room.GetNeighbourPositions()) {
                if(context.createdRooms.TryGetValue(pos, out Room neighbour)) {
                    room.Connect(neighbour);
                }
            }
        }
    }

    private void DrawWalls(Room room) {
        float left = -room.RoomSize.x / 2f + room.RoomPos.x;
        float right = room.RoomSize.x / 2f + room.RoomPos.x;
        float top = room.RoomSize.y / 2f + room.RoomPos.y;
        float bottom = -room.RoomSize.y / 2f + room.RoomPos.y;

        for(int i = 0; i <= room.RoomSize.x; i++) {
            for(int j = 0; j <= room.RoomSize.y; j++) {
                Vector2 tilePos = new Vector2(left + i, top - j);
                Vector2Int tile = Vector2Int.RoundToInt(tilePos);

                bool isEdgeTile =
                    Mathf.Approximately(tilePos.x, left) ||
                    Mathf.Approximately(tilePos.x, right) ||
                    Mathf.Approximately(tilePos.y, top) ||
                    Mathf.Approximately(tilePos.y, bottom);

                if(isEdgeTile && !IsDoorTile(room, tile)) {
                    tilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), context.wallTile);
                }
            }
        }
    }

    private bool IsDoorTile(Room room, Vector2Int tile) {
        int x = room.RoomPos.x;
        int y = room.RoomPos.y;
        int halfWidth = room.RoomSize.x / 2;
        int halfHeight = room.RoomSize.y / 2;

        var doorOffsets = new Dictionary<Direction, (bool isOpen, Vector2Int center)> {
            { Direction.North, (room.DoorTop, new Vector2Int(x, y + halfHeight)) },
            { Direction.South, (room.DoorBottom, new Vector2Int(x, y - halfHeight)) },
            { Direction.West, (room.DoorLeft, new Vector2Int(x - halfWidth, y)) },
            { Direction.East, (room.DoorRight, new Vector2Int(x + halfWidth, y)) }
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
