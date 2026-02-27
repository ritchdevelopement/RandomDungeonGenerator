using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FogOfWarManager : DungeonTaskBase {
    public static FogOfWarManager Instance { get; private set; }

    [SerializeField] private Color fogColor = Color.black;
    [SerializeField] private int fogSortingOrder = 10;

    private const int PixelsPerUnit = 1;

    private readonly Dictionary<Room, RoomFog> roomFogs = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public override void Execute() {
        roomFogs.Clear();
        Transform fogContainer = CreateFogContainer();
        Sprite unitWhiteSprite = CreateUnitWhiteSprite();
        foreach (Room room in context.createdRooms.Values) {
            List<GameObject> overlays = CreateAllFogOverlays(room, fogContainer, unitWhiteSprite);
            roomFogs[room] = new RoomFog(room, overlays);
        }

        RevealRoom(context.playerSpawnRoom);
    }

    private List<GameObject> CreateAllFogOverlays(Room room, Transform parent, Sprite sprite) {
        RectInt bounds = room.Bounds;
        int extendX = room.RoomSize.x;
        int extendY = room.RoomSize.y;
        Vector2Int center = room.Center;

        bool hasNorth = room.Neighbors.ContainsKey(Direction.North);
        bool hasSouth = room.Neighbors.ContainsKey(Direction.South);
        bool hasEast = room.Neighbors.ContainsKey(Direction.East);
        bool hasWest = room.Neighbors.ContainsKey(Direction.West);

        bool hasNorthEast = context.createdRooms.ContainsKey(center + new Vector2Int(extendX, extendY));
        bool hasNorthWest = context.createdRooms.ContainsKey(center + new Vector2Int(-extendX, extendY));
        bool hasSouthEast = context.createdRooms.ContainsKey(center + new Vector2Int(extendX, -extendY));
        bool hasSouthWest = context.createdRooms.ContainsKey(center + new Vector2Int(-extendX, -extendY));

        // Edge overlays cover empty space permanently — not included in RoomFog
        if (!hasNorth) CreateFogOverlay(NorthSideBounds(bounds, extendY), $"FogN_{center}", parent, sprite);
        if (!hasSouth) CreateFogOverlay(SouthSideBounds(bounds, extendY), $"FogS_{center}", parent, sprite);
        if (!hasEast) CreateFogOverlay(EastSideBounds(bounds, extendX), $"FogE_{center}", parent, sprite);
        if (!hasWest) CreateFogOverlay(WestSideBounds(bounds, extendX), $"FogW_{center}", parent, sprite);
        if (!hasNorthEast) CreateFogOverlay(NorthEastCornerBounds(bounds, extendX, extendY), $"FogNE_{center}", parent, sprite);
        if (!hasNorthWest) CreateFogOverlay(NorthWestCornerBounds(bounds, extendX, extendY), $"FogNW_{center}", parent, sprite);
        if (!hasSouthEast) CreateFogOverlay(SouthEastCornerBounds(bounds, extendX, extendY), $"FogSE_{center}", parent, sprite);
        if (!hasSouthWest) CreateFogOverlay(SouthWestCornerBounds(bounds, extendX, extendY), $"FogSW_{center}", parent, sprite);

        // Only the main overlay is tracked in RoomFog — removed when room is revealed
        return new List<GameObject> { CreateFogOverlay(bounds, $"Fog_{center}", parent, sprite) };
    }

    private GameObject CreateFogOverlay(RectInt bounds, string name, Transform parent, Sprite sprite) {
        GameObject fogOverlay = new(name);
        fogOverlay.transform.parent = parent;
        SpriteRenderer spriteRenderer = fogOverlay.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = fogColor;
        spriteRenderer.sortingOrder = fogSortingOrder;
        fogOverlay.transform.position = CalculateWorldCenter(bounds);
        fogOverlay.transform.localScale = CalculateWorldScale(bounds);
        return fogOverlay;
    }

    private static RectInt NorthSideBounds(RectInt r, int extendY) => new(r.xMin, r.yMax, r.width, extendY);
    private static RectInt SouthSideBounds(RectInt r, int extendY) => new(r.xMin, r.yMin - extendY, r.width, extendY);
    private static RectInt EastSideBounds(RectInt r, int extendX) => new(r.xMax, r.yMin, extendX, r.height);
    private static RectInt WestSideBounds(RectInt r, int extendX) => new(r.xMin - extendX, r.yMin, extendX, r.height);
    private static RectInt NorthEastCornerBounds(RectInt r, int extendX, int extendY) => new(r.xMax, r.yMax, extendX, extendY);
    private static RectInt NorthWestCornerBounds(RectInt r, int extendX, int extendY) => new(r.xMin - extendX, r.yMax, extendX, extendY);
    private static RectInt SouthEastCornerBounds(RectInt r, int extendX, int extendY) => new(r.xMax, r.yMin - extendY, extendX, extendY);
    private static RectInt SouthWestCornerBounds(RectInt r, int extendX, int extendY) => new(r.xMin - extendX, r.yMin - extendY, extendX, extendY);

    private static Vector3 CalculateWorldCenter(RectInt bounds) {
        return new Vector3(
            bounds.xMin + bounds.width / 2f,
            bounds.yMin + bounds.height / 2f,
            0f
        );
    }

    private static Vector3 CalculateWorldScale(RectInt bounds) {
        return new Vector3(bounds.width, bounds.height, 1f);
    }

    private Transform CreateFogContainer() {
        GameObject fogContainer = new("FogOfWar");
        fogContainer.transform.parent = context.dungeonGameObject.transform;
        return fogContainer.transform;
    }

    private Sprite CreateUnitWhiteSprite() {
        Texture2D whiteTexture = new(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        Vector2 centerPivot = new(0.5f, 0.5f);
        return Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), centerPivot, PixelsPerUnit);
    }

    public void RevealRoom(Room room) {
        if (room == null || !roomFogs.TryGetValue(room, out RoomFog roomFog)) return;
        roomFog.Reveal();
        roomFogs.Remove(room);
    }
}
