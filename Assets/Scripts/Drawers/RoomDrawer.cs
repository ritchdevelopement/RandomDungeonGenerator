using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDrawer : DungeonTaskBase {
    [SerializeField] private TileBase defaultTile;
    [SerializeField] private WallTileSet[] wallTileSets;
    [SerializeField] private TileBase[] floorTiles;
    private Tilemap wallTilemap;
    private Tilemap floorTilemap;

    public override void Execute() {
        if (defaultTile == null) {
            throw new MissingReferenceException("No defaultTile assigned to RoomDrawer.");
        }

        DrawRooms();
    }

    private void DrawRooms() {
        CreateWallTilemap();
        CreateFloorTilemap();

        wallTilemap.color = wallTileSets is { Length: > 0 } ? Color.white : Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        floorTilemap.color = floorTiles is { Length: > 0 } ? Color.white : Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);

        WallTileSet wallTileSet = wallTileSets is { Length: > 0 }
            ? wallTileSets[Random.Range(0, wallTileSets.Length)]
            : null;
        TileBase floorTile = floorTiles is { Length: > 0 }
            ? floorTiles[Random.Range(0, floorTiles.Length)]
            : defaultTile;

        foreach (Room room in context.createdRooms.Values) {
            DrawWalls(room, wallTileSet);
            DrawOuterWallRows(room, wallTileSet);
            DrawFloor(room, floorTile);
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

    private void DrawWalls(Room room, WallTileSet wallTileSet) {
        RectInt roomBounds = room.Bounds;

        for (int x = roomBounds.xMin; x < roomBounds.xMax; x++) {
            for (int y = roomBounds.yMin; y < roomBounds.yMax; y++) {
                Vector2Int position = new Vector2Int(x, y);

                if (!room.IsWallTile(position)) {
                    continue;
                }

                TileBase tile = SelectWallTile(roomBounds, position, wallTileSet);
                wallTilemap.SetTile(new Vector3Int(x, y), tile);
            }
        }
    }

    private TileBase SelectWallTile(RectInt bounds, Vector2Int position, WallTileSet wallTileSet) {
        if (wallTileSet == null) {
            return defaultTile;
        }

        bool isTopEdge = position.y == bounds.yMax - 1;
        bool isBottomEdge = position.y == bounds.yMin;
        bool isLeftEdge = position.x == bounds.xMin;
        bool isRightEdge = position.x == bounds.xMax - 1;

        if (isTopEdge && isLeftEdge) {
            return wallTileSet.cornerTopLeft != null ? wallTileSet.cornerTopLeft : defaultTile;
        }
        if (isTopEdge && isRightEdge) {
            return wallTileSet.cornerTopRight != null ? wallTileSet.cornerTopRight : defaultTile;
        }
        if (isBottomEdge && isLeftEdge) {
            return wallTileSet.cornerBottomLeft != null ? wallTileSet.cornerBottomLeft : defaultTile;
        }
        if (isBottomEdge && isRightEdge) {
            return wallTileSet.cornerBottomRight != null ? wallTileSet.cornerBottomRight : defaultTile;
        }

        if (isTopEdge) {
            return wallTileSet.wallTop != null ? wallTileSet.wallTop : defaultTile;
        }
        if (isBottomEdge) {
            return wallTileSet.wallBottom != null ? wallTileSet.wallBottom : defaultTile;
        }
        if (isLeftEdge) {
            return wallTileSet.wallVerticalLeft != null ? wallTileSet.wallVerticalLeft : defaultTile;
        }
        if (isRightEdge) {
            return wallTileSet.wallVerticalRight != null ? wallTileSet.wallVerticalRight : defaultTile;
        }

        return defaultTile;
    }

    private void DrawOuterWallRows(Room room, WallTileSet wallTileSet) {
        if (wallTileSet == null) {
            return;
        }

        RectInt bounds = room.Bounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            Vector2Int outerBottomPosition = new Vector2Int(x, bounds.yMin - 1);
            if (!IsOccupiedByAnyRoom(outerBottomPosition)) {
                TileBase tile = SelectOuterBottomTile(bounds, outerBottomPosition, wallTileSet);
                wallTilemap.SetTile(new Vector3Int(x, bounds.yMin - 1), tile);
            }

            Vector2Int outerTopPosition = new Vector2Int(x, bounds.yMax);
            if (!IsOccupiedByAnyRoom(outerTopPosition)) {
                TileBase tile = SelectOuterTopTile(bounds, outerTopPosition, wallTileSet);
                wallTilemap.SetTile(new Vector3Int(x, bounds.yMax), tile);
            }
        }
    }

    private TileBase SelectOuterBottomTile(RectInt bounds, Vector2Int position, WallTileSet wallTileSet) {
        if (position.x == bounds.xMin) {
            return wallTileSet.outerCornerBottomLeft != null ? wallTileSet.outerCornerBottomLeft : defaultTile;
        }
        if (position.x == bounds.xMax - 1) {
            return wallTileSet.outerCornerBottomRight != null ? wallTileSet.outerCornerBottomRight : defaultTile;
        }
        return wallTileSet.outerWallBottom != null ? wallTileSet.outerWallBottom : defaultTile;
    }

    private TileBase SelectOuterTopTile(RectInt bounds, Vector2Int position, WallTileSet wallTileSet) {
        if (position.x == bounds.xMin) {
            return wallTileSet.outerCornerTopLeft != null ? wallTileSet.outerCornerTopLeft : defaultTile;
        }
        if (position.x == bounds.xMax - 1) {
            return wallTileSet.outerCornerTopRight != null ? wallTileSet.outerCornerTopRight : defaultTile;
        }
        return wallTileSet.outerWallTop != null ? wallTileSet.outerWallTop : defaultTile;
    }

    private bool IsOccupiedByAnyRoom(Vector2Int position) {
        foreach (Room room in context.createdRooms.Values) {
            if (room.Bounds.Contains(position)) {
                return true;
            }
        }
        return false;
    }

    private void DrawFloor(Room room, TileBase floorTile) {
        RectInt roomBounds = room.Bounds;

        for (int x = roomBounds.xMin; x < roomBounds.xMax; x++) {
            for (int y = roomBounds.yMin; y < roomBounds.yMax; y++) {
                Vector2Int position = new Vector2Int(x, y);

                bool isBottomWallWithoutCorners = position.y == roomBounds.yMin
                    && position.x != roomBounds.xMin
                    && position.x != roomBounds.xMax - 1;
                if (!room.IsFloorTile(position) && !isBottomWallWithoutCorners) {
                    continue;
                }

                floorTilemap.SetTile(new Vector3Int(x, y), floorTile);
            }
        }
    }
}
