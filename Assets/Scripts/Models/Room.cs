using System.Collections.Generic;
using UnityEngine;

public class Room {
    public Dictionary<Direction, Room> Neighbors { get; } = new();
    public Dictionary<Direction, Door> Doors { get; } = new();
    public Vector2Int Center { get; }
    public Vector2Int RoomSize { get; }

    public Room(Vector2Int roomSize, Vector2Int center) {
        RoomSize = roomSize;
        Center = center;
    }

    public List<Vector2Int> GetNeighbourPositions() {
        int spacingX = RoomSize.x + 1;
        int spacingY = RoomSize.y + 1;
        return new List<Vector2Int> {
            new Vector2Int(Center.x - spacingX, Center.y),
            new Vector2Int(Center.x + spacingX, Center.y),
            new Vector2Int(Center.x, Center.y + spacingY),
            new Vector2Int(Center.x, Center.y - spacingY)
        };
    }

    public void Connect(Room neighbor, Door door, Direction dir) {
        Direction opposite = dir.Opposite();

        Neighbors[dir] = neighbor;
        neighbor.Neighbors[opposite] = this;
        Doors[dir] = door;
        neighbor.Doors[opposite] = door;
    }

    public Direction GetDirectionTo(Room neighbor) {
        Vector2Int delta = neighbor.Center - Center;

        return delta switch {
            { x: 0, y: > 0 } => Direction.North,
            { x: 0, y: < 0 } => Direction.South,
            { x: > 0, y: 0 } => Direction.East,
            { x: < 0, y: 0 } => Direction.West,
            _ => throw new System.InvalidOperationException($"Rooms {this} and {neighbor} are not directly adjacent!")
        };
    }

    public bool IsDoorTile(Vector2Int tile) {
        foreach(var door in Doors.Values) {
            if(door.TilePositions.Contains(tile)) {
                return true;
            }
        }
        return false;
    }

    public bool HasDoor(Direction direction) => Doors.ContainsKey(direction);
}
