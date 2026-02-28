using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDrawer : DungeonTaskBase {
    public TileBase wallTile;
    private Tilemap dungeonTilemap;

    public override void Execute() {

        if (wallTile == null) {
            throw new MissingReferenceException($"No wall tile set for rooms: {wallTile}");
        }

        DrawRooms();
    }

    private void DrawRooms() {
        CreateTilemap();

        foreach (Room room in context.createdRooms.Values) {
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
        RectInt roomBounds = room.Bounds;

        for (int x = roomBounds.xMin; x < roomBounds.xMax; x++) {
            for (int y = roomBounds.yMin; y < roomBounds.yMax; y++) {
                Vector2Int position = new Vector2Int(x, y);

                if (!room.IsWallTile(position)) {
                    continue;
                }

                dungeonTilemap.SetTile(new Vector3Int(x, y), wallTile);
            }
        }
    }
}
