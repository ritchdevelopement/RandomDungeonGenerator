using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDrawer : DungeonTaskBase {
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
        GameObject tilemapGameObject = new GameObject("Rooms");
        tilemapGameObject.transform.parent = context.dungeonGameObject.transform;

        dungeonTilemap = tilemapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;
    }

    private void DrawWalls(Room room) {
        int halfWidth = room.RoomSize.x / 2;
        int halfHeight = room.RoomSize.y / 2;

        int left = room.Center.x - halfWidth;
        int right = room.Center.x + halfWidth;
        int top = room.Center.y + halfHeight;
        int bottom = room.Center.y - halfHeight;

        for(int x = left; x <= right; x++) {
            for(int y = bottom; y <= top; y++) {
                bool isEdgeTile = 
                    x == left || 
                    x == right || 
                    y == top || 
                    y == bottom;

                if(!isEdgeTile || room.IsDoorTile(new Vector2Int(x, y))) {
                    continue;
                }

                dungeonTilemap.SetTile(new Vector3Int(x, y), context.wallTile);
            }
        }
    }
}
