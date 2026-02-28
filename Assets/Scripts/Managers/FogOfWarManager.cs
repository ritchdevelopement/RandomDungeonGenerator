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
        SyncCameraBackground();
        roomFogs.Clear();
        Transform fogContainer = CreateFogContainer();
        Sprite whiteSquareSprite = CreateWhiteSquareSprite();
        foreach (Room room in context.createdRooms.Values) {
            roomFogs[room] = CreateRoomFog(room, fogContainer, whiteSquareSprite);
        }

        RevealRoom(context.playerSpawnRoom);
    }

    private void SyncCameraBackground() {
        Camera mainCamera = Camera.main;
        if (mainCamera != null) {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = fogColor;
        }
    }

    private RoomFog CreateRoomFog(Room room, Transform parent, Sprite sprite) {
        GameObject fogOverlay = CreateFogOverlay(room.Bounds, $"Fog_{room.Center}", parent, sprite);
        AttachRevealTrigger(fogOverlay, room);
        return new RoomFog(room, new List<GameObject> { fogOverlay });
    }

    private static void AttachRevealTrigger(GameObject fogOverlay, Room room) {
        BoxCollider2D revealCollider = fogOverlay.AddComponent<BoxCollider2D>();
        revealCollider.isTrigger = true;

        FogRevealTrigger revealTrigger = fogOverlay.AddComponent<FogRevealTrigger>();
        revealTrigger.Initialize(room);
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

    private Sprite CreateWhiteSquareSprite() {
        Texture2D whiteTexture = new(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        Vector2 centerPivot = new(0.5f, 0.5f);
        return Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), centerPivot, PixelsPerUnit);
    }

    public void RevealRoom(Room room) {
        if (room == null || !roomFogs.TryGetValue(room, out RoomFog roomFog)) {
            return;
        }
        roomFog.Reveal();
        roomFogs.Remove(room);
    }
}
