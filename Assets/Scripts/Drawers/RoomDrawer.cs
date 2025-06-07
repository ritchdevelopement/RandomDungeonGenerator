using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDrawer : DungeonTaskBase {
    public TileBase wallTile;
    private Tilemap dungeonTilemap;

    public override void Execute() {

        if(wallTile == null) {
            throw new MissingReferenceException($"No wall tile set for rooms: {wallTile}");
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

        RoomManager roomManager = tilemapGameObject.AddComponent<RoomManager>();
        roomManager.SetContext(context);

        Rigidbody2D rb = tilemapGameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        dungeonTilemap = tilemapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;

        TilemapCollider2D tilemapCollider = tilemapGameObject.AddComponent<TilemapCollider2D>();
        CompositeCollider2D compositeCollider = tilemapGameObject.AddComponent<CompositeCollider2D>();
        compositeCollider.sharedMaterial = context.frictionlessMaterial;
        tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
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

                dungeonTilemap.SetTile(new Vector3Int(x, y), wallTile);
            }
        }
    }
}
