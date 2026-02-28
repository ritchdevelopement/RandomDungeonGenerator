using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDrawer : DungeonTaskBase {
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase floorTile;
    private Tilemap wallTilemap;
    private Tilemap floorTilemap;

    public override void Execute() {
        if (wallTile == null) {
            throw new MissingReferenceException($"No wall tile set for rooms: {wallTile}");
        }

        if (floorTile == null) {
            throw new MissingReferenceException($"No floor tile set for rooms: {floorTile}");
        }

        DrawRooms();
    }

    private void DrawRooms() {
        CreateWallTilemap();
        CreateFloorTilemap();

        foreach (Room room in context.createdRooms.Values) {
            DrawWalls(room);
            DrawFloor(room);
        }
    }

    private void CreateWallTilemap() {
        GameObject tilemapGameObject = new GameObject("Rooms");
        tilemapGameObject.transform.parent = context.dungeonGameObject.transform;

        RoomManager roomManager = tilemapGameObject.AddComponent<RoomManager>();
        roomManager.SetContext(context);

        Rigidbody2D rb = tilemapGameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        wallTilemap = tilemapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;

        TilemapCollider2D tilemapCollider = tilemapGameObject.AddComponent<TilemapCollider2D>();
        CompositeCollider2D compositeCollider = tilemapGameObject.AddComponent<CompositeCollider2D>();
        compositeCollider.sharedMaterial = context.frictionlessMaterial;
        tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
    }

    private void CreateFloorTilemap() {
        GameObject floorGameObject = new GameObject("Floor");
        floorGameObject.transform.parent = context.dungeonGameObject.transform;

        floorTilemap = floorGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = floorGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;
        tilemapRenderer.sortingOrder = -1;
    }

    private void DrawWalls(Room room) {
        RectInt roomBounds = room.Bounds;

        for (int x = roomBounds.xMin; x < roomBounds.xMax; x++) {
            for (int y = roomBounds.yMin; y < roomBounds.yMax; y++) {
                Vector2Int position = new Vector2Int(x, y);

                if (!room.IsWallTile(position)) {
                    continue;
                }

                wallTilemap.SetTile(new Vector3Int(x, y), wallTile);
            }
        }
    }

    private void DrawFloor(Room room) {
        RectInt roomBounds = room.Bounds;

        for (int x = roomBounds.xMin; x < roomBounds.xMax; x++) {
            for (int y = roomBounds.yMin; y < roomBounds.yMax; y++) {
                Vector2Int position = new Vector2Int(x, y);

                if (!room.IsFloorTile(position)) {
                    continue;
                }

                floorTilemap.SetTile(new Vector3Int(x, y), floorTile);
            }
        }
    }
}
