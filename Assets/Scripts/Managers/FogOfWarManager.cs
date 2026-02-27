using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : DungeonTaskBase {
    public static FogOfWarManager Instance { get; private set; }

    [SerializeField] private Color fogColor = Color.black;
    [SerializeField] private int fogSortingOrder = 10;

    private const int PixelsPerUnit = 1;

    private readonly Dictionary<Room, GameObject> fogOverlayByRoom = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public override void Execute() {
        fogOverlayByRoom.Clear();

        Transform fogContainer = CreateFogContainer();
        Sprite unitWhiteSprite = CreateUnitWhiteSprite();

        foreach (Room room in context.createdRooms.Values) {
            CreateFogOverlay(room, fogContainer, unitWhiteSprite);
        }
    }

    private Transform CreateFogContainer() {
        GameObject fogContainer = new("FogOfWar");
        fogContainer.transform.parent = context.dungeonGameObject.transform;
        return fogContainer.transform;
    }

    private void CreateFogOverlay(Room room, Transform parent, Sprite sprite) {
        GameObject fogOverlay = new($"Fog_{room.Center}");
        fogOverlay.transform.parent = parent;

        SpriteRenderer spriteRenderer = fogOverlay.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = fogColor;
        spriteRenderer.sortingOrder = fogSortingOrder;

        fogOverlay.transform.position = CalculateRoomWorldCenter(room);
        fogOverlay.transform.localScale = CalculateRoomWorldScale(room);

        fogOverlayByRoom[room] = fogOverlay;
    }

    private Vector3 CalculateRoomWorldCenter(Room room) {
        RectInt bounds = room.Bounds;
        return new Vector3(
            bounds.xMin + bounds.width / 2f,
            bounds.yMin + bounds.height / 2f,
            0f
        );
    }

    private Vector3 CalculateRoomWorldScale(Room room) {
        return new Vector3(room.Bounds.width, room.Bounds.height, 1f);
    }

    private Sprite CreateUnitWhiteSprite() {
        Texture2D whiteTexture = new(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();

        Vector2 centerPivot = new(0.5f, 0.5f);
        return Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), centerPivot, PixelsPerUnit);
    }

    public void RevealRoom(Room room) {
        if (room == null) return;

        if (fogOverlayByRoom.TryGetValue(room, out GameObject fogOverlay)) {
            fogOverlay.SetActive(false);
        }
    }
}
