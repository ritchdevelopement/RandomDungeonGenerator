using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorDrawer : DungeonTaskBase {
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private Sprite[] doorSprites;
    [SerializeField] private TileBase[] eastDoorTiles;
    [SerializeField] private TileBase[] westDoorTiles;
    [SerializeField] private GameObject verticalDoorPanelPrefab;
    [SerializeField] private int[] additionalDoorPrefabYOffsets;
    private Transform doorsParent;
    private Grid dungeonGrid;

    public override void Execute() {
        if (doorPrefab == null) {
            Debug.LogError("Door prefab not assigned to DoorDrawer!");
            return;
        }

        GameObject doorsGameObject = new GameObject("Doors");
        doorsGameObject.transform.SetParent(context.dungeonGameObject.transform);
        doorsParent = doorsGameObject.transform;
        dungeonGrid = context.dungeonGameObject.GetComponent<Grid>();
        doorsGameObject.AddComponent<DoorManager>();

        DrawDoors();
    }

    private void DrawDoors() {
        Color doorColor = doorSprites is { Length: > 0 } ? Color.white : Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);

        foreach (Door door in context.createdDoors) {
            bool isVertical = door.Direction == Direction.East || door.Direction == Direction.West;
            if (isVertical) {
                DrawVerticalDoor(door);
            } else {
                DrawHorizontalDoor(door, doorColor);
            }
        }
    }

    private void DrawHorizontalDoor(Door door, Color doorColor) {
        Vector3 worldPos = GetDoorYOffsetWorldCenter(door, 0);
        GameObject doorGameObject = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorsParent);

        SetupHorizontalDoorSprite(doorGameObject, door, doorColor);

        if (doorGameObject.TryGetComponent(out BoxCollider2D boxCollider)) {
            boxCollider.size = new Vector2(door.Size.x, 1f);
        }

        DoorController doorController = doorGameObject.GetComponent<DoorController>();
        doorController.Initialize(door, door.RoomA, door.RoomB);
    }

    private void SetupHorizontalDoorSprite(GameObject doorGameObject, Door door, Color doorColor) {
        if (!doorGameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) {
            return;
        }

        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.size = new Vector2(door.Size.x, 1f);
        spriteRenderer.color = doorColor;

        if (doorSprites == null || doorSprites.Length == 0) {
            return;
        }

        spriteRenderer.sprite = doorSprites[Random.Range(0, doorSprites.Length)];
    }

    private void DrawVerticalDoor(Door door) {
        Vector3 worldPos = GetDoorWorldCenter(door);
        GameObject masterDoor = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorsParent);

        if (masterDoor.TryGetComponent(out SpriteRenderer masterRenderer)) {
            masterRenderer.drawMode = SpriteDrawMode.Tiled;
            masterRenderer.size = new Vector2(door.Size.x, door.Size.y);
            masterRenderer.enabled = false;
        }

        if (masterDoor.TryGetComponent(out BoxCollider2D boxCollider)) {
            boxCollider.size = new Vector2(door.Size.x, door.Size.y);
        }

        PlaceVerticalDoorTilesAndPanels(door, masterDoor);

        DoorController doorController = masterDoor.GetComponent<DoorController>();
        doorController.Initialize(door, door.RoomA, door.RoomB);
    }

    private void PlaceVerticalDoorTilesAndPanels(Door door, GameObject masterDoor) {
        for (int yOffset = 0; yOffset < door.Size.y; yOffset++) {
            TileBase eastTile = eastDoorTiles != null && yOffset < eastDoorTiles.Length ? eastDoorTiles[yOffset] : null;
            TileBase westTile = westDoorTiles != null && yOffset < westDoorTiles.Length ? westDoorTiles[yOffset] : null;

            PlaceVerticalDoorTile(door.MinBounds.x, door.MinBounds.y + yOffset, eastTile);
            PlaceVerticalDoorTile(door.MaxBounds.x, door.MinBounds.y + yOffset, westTile);

            bool isNullSlot = eastTile == null;
            bool isAdditionalOffset = IsAdditionalDoorPrefabOffset(yOffset);

            if ((isNullSlot || isAdditionalOffset) && verticalDoorPanelPrefab != null) {
                SpawnVerticalDoorPanel(door, masterDoor, yOffset);
            }
        }
    }

    private void PlaceVerticalDoorTile(int x, int y, TileBase tile) {
        if (tile == null) {
            return;
        }

        context.wallTilemap.SetTile(new Vector3Int(x, y), tile);
    }

    private bool IsAdditionalDoorPrefabOffset(int yOffset) {
        if (additionalDoorPrefabYOffsets == null) {
            return false;
        }

        foreach (int offset in additionalDoorPrefabYOffsets) {
            if (offset == yOffset) {
                return true;
            }
        }

        return false;
    }

    private void SpawnVerticalDoorPanel(Door door, GameObject masterDoor, int yOffset) {
        Vector3 panelWorldPos = GetDoorYOffsetWorldCenter(door, yOffset);
        GameObject panel = Instantiate(verticalDoorPanelPrefab, panelWorldPos, Quaternion.identity, masterDoor.transform);

        if (panel.TryGetComponent(out SpriteRenderer panelRenderer)) {
            panelRenderer.drawMode = SpriteDrawMode.Tiled;
            panelRenderer.size = new Vector2(door.Size.x, 1f);
            panelRenderer.sortingOrder = IsLastAdditionalDoorPrefabOffset(yOffset) ? -1 : 0;
        }
    }

    private bool IsLastAdditionalDoorPrefabOffset(int yOffset) {
        if (additionalDoorPrefabYOffsets == null || additionalDoorPrefabYOffsets.Length == 0) {
            return false;
        }

        return additionalDoorPrefabYOffsets[^1] == yOffset;
    }

    private Vector3 GetDoorYOffsetWorldCenter(Door door, int yOffset) {
        int tileY = door.MinBounds.y + yOffset;
        Vector3 minWorld = dungeonGrid.CellToWorld(new Vector3Int(door.MinBounds.x, tileY));
        Vector3 maxWorld = dungeonGrid.CellToWorld(new Vector3Int(door.MaxBounds.x + 1, tileY + 1));
        return (minWorld + maxWorld) * 0.5f;
    }

    private Vector3 GetDoorWorldCenter(Door door) {
        Vector3 minWorld = dungeonGrid.CellToWorld(new Vector3Int(door.MinBounds.x, door.MinBounds.y));
        Vector3 maxWorld = dungeonGrid.CellToWorld(new Vector3Int(door.MaxBounds.x + 1, door.MaxBounds.y + 1));

        return (minWorld + maxWorld) * 0.5f;
    }
}
