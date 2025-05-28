using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDrawer : DungeonTask {
    private Tilemap dungeonTilemap;

    public override void Execute() {

        if(context.wallTile == null) {
            throw new MissingReferenceException($"No wall tile set for rooms: {context.wallTile}");
        }

        DrawRooms();
    }

    private void DrawRooms() {
        CreateTilemap();

        foreach(Room room in context.createdRooms.Values) {
            DrawWalls(room);
        }
    }
    private void CreateTilemap() {
        GameObject gridGameObject = new GameObject("Dungeon");
        Grid dungeonGrid = gridGameObject.AddComponent<Grid>();
        dungeonGrid.cellSize = new Vector3Int(1, 1, 0);

        GameObject tilemapGameObject = new GameObject("Rooms");
        tilemapGameObject.transform.parent = gridGameObject.transform;

        dungeonTilemap = tilemapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;
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
                    dungeonTilemap.SetTile(new Vector3Int(tile.x, tile.y), context.wallTile);
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
