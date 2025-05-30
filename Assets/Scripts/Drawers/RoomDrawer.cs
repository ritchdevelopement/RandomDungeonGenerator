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
        float left = -room.RoomSize.x / 2f + room.Center.x;
        float right = room.RoomSize.x / 2f + room.Center.x;
        float top = room.RoomSize.y / 2f + room.Center.y;
        float bottom = -room.RoomSize.y / 2f + room.Center.y;

        for(int i = 0; i <= room.RoomSize.x; i++) {
            for(int j = 0; j <= room.RoomSize.y; j++) {
                Vector2 tilePos = new Vector2(left + i, top - j);
                Vector2Int tile = Vector2Int.RoundToInt(tilePos);

                bool isEdgeTile =
                    Mathf.Approximately(tilePos.x, left) ||
                    Mathf.Approximately(tilePos.x, right) ||
                    Mathf.Approximately(tilePos.y, top) ||
                    Mathf.Approximately(tilePos.y, bottom);

                if(!isEdgeTile) { 
                    continue; 
                }

                if(room.IsDoorTile(tile)) {
                    dungeonTilemap.SetTile(new Vector3Int(tile.x, tile.y), context.doorTile);
                } else {
                    dungeonTilemap.SetTile(new Vector3Int(tile.x, tile.y), context.wallTile);
                }
            }
        }
    }
}
