using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorDrawer : DungeonTaskBase {
    [SerializeField] private GameObject doorPrefab;
    private Transform doorsParent;
    private Grid dungeonGrid;

    public override void Execute() {
        GameObject doorsGameObject = new GameObject("Doors");
        doorsGameObject.transform.SetParent(context.dungeonGameObject.transform);
        doorsParent = doorsGameObject.transform;
        dungeonGrid = context.dungeonGameObject.GetComponent<Grid>();

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
    }

    private Vector3 GetDoorWorldCenter(Door door) {
        List<Vector2Int> tilePositions = door.TilePositions;
        int minX = tilePositions.Min(pos => pos.x);
        int maxX = tilePositions.Max(pos => pos.x);
        int minY = tilePositions.Min(pos => pos.y);
        int maxY = tilePositions.Max(pos => pos.y);

        Vector3 minWorld = dungeonGrid.CellToWorld(new Vector3Int(minX, minY));
        Vector3 maxWorld = dungeonGrid.CellToWorld(new Vector3Int(maxX + 1, maxY + 1));

        return (minWorld + maxWorld) * 0.5f;
    }
}
