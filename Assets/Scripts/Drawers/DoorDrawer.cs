using UnityEngine;

public class DoorDrawer : DungeonTaskBase {
    [SerializeField] private GameObject doorPrefab;
    private Transform doorsParent;
    private Grid dungeonGrid;

    public override void Execute() {
        if(doorPrefab == null) {
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
        foreach(Door door in context.createdDoors) {
            DrawDoor(door);
        }
    }

    private void DrawDoor(Door door) {
        Vector3 worldPos = GetDoorWorldCenter(door);
        GameObject doorGameObject = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorsParent);

        if(doorGameObject.TryGetComponent(out SpriteRenderer renderer)) {
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.size = door.Size;
        }

        if(doorGameObject.TryGetComponent(out BoxCollider2D boxCollider)) {
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
