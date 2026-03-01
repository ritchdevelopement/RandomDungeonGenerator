using UnityEngine;

public class DoorDrawer : DungeonTaskBase {
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private Sprite[] doorSprites;
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
            DrawDoor(door, doorColor);
        }
    }

    private void DrawDoor(Door door, Color doorColor) {
        Vector3 worldPos = GetDoorWorldCenter(door);
        GameObject doorGameObject = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorsParent);

        if (doorGameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = door.Size;
            spriteRenderer.color = doorColor;

            if (doorSprites != null && doorSprites.Length > 0) {
                spriteRenderer.sprite = doorSprites[Random.Range(0, doorSprites.Length)];
            }
        }

        if (doorGameObject.TryGetComponent(out BoxCollider2D boxCollider)) {
            boxCollider.size = new Vector2(door.Size.x, door.Size.y);
        }

        DoorController doorController = doorGameObject.GetComponent<DoorController>();
        doorController.Initialize(door, door.RoomA, door.RoomB);
    }

    private Vector3 GetDoorWorldCenter(Door door) {
        Vector3 minWorld = dungeonGrid.CellToWorld(new Vector3Int(door.MinBounds.x, door.MinBounds.y));
        Vector3 maxWorld = dungeonGrid.CellToWorld(new Vector3Int(door.MaxBounds.x + 1, door.MaxBounds.y + 1));

        return (minWorld + maxWorld) * 0.5f;
    }
}
