using System.Collections.Generic;
using UnityEngine;

public class Room {
    public Dictionary<Direction, Room> Neighbors { get; } = new();
    public Dictionary<Direction, Door> Doors { get; } = new();
    public Vector2Int Center { get; }
    public Vector2Int RoomSize { get; }
    public RectInt Bounds { get; }

    public Room(Vector2Int roomSize, Vector2Int center) {
        Vector2Int originalSize = roomSize;
        RoomSize = EnsureOddSize(roomSize);
        Center = center;
        Bounds = CalculateBounds();

        if (RoomSize != originalSize) {
            Debug.LogWarning($"Room at {Center}: size corrected from {originalSize} to {RoomSize} (odd numbers required for correct room center).");
        }
    }

    public static Vector2Int EnsureOddSize(Vector2Int size) {
        return new Vector2Int(
            size.x % 2 == 0 ? Mathf.Max(1, size.x + 1) : Mathf.Max(1, size.x),
            size.y % 2 == 0 ? Mathf.Max(1, size.y + 1) : Mathf.Max(1, size.y)
        );
    }

    public Vector2Int GetNeighborCenter(Direction dir, Vector2Int neighborSize) {
        int halfOwnX = RoomSize.x / 2;
        int halfOwnY = RoomSize.y / 2;
        int halfNeighborX = neighborSize.x / 2;
        int halfNeighborY = neighborSize.y / 2;

        return dir switch {
            Direction.North => new Vector2Int(Center.x, Center.y + halfOwnY + halfNeighborY + 1),
            Direction.South => new Vector2Int(Center.x, Center.y - halfOwnY - halfNeighborY - 1),
            Direction.East => new Vector2Int(Center.x + halfOwnX + halfNeighborX + 1, Center.y),
            Direction.West => new Vector2Int(Center.x - halfOwnX - halfNeighborX - 1, Center.y),
            _ => throw new System.ArgumentOutOfRangeException(nameof(dir))
        };
    }

    public void Connect(Room neighbor, Door door, Direction dir) {
        Direction opposite = dir.Opposite();

        Neighbors.Add(dir, neighbor);
        neighbor.Neighbors.Add(opposite, this);
        Doors.Add(dir, door);
        neighbor.Doors.Add(opposite, door);
    }

    public Direction GetDirectionTo(Room neighbor) {
        Vector2Int delta = neighbor.Center - Center;

        return delta switch { { x: 0, y: > 0 } => Direction.North, { x: 0, y: < 0 } => Direction.South, { x: > 0, y: 0 } => Direction.East, { x: < 0, y: 0 } => Direction.West,
            _ => throw new System.InvalidOperationException($"Rooms {this} and {neighbor} are not directly adjacent!")
        };
    }

    private RectInt CalculateBounds() {
        return new RectInt(
            Center.x - RoomSize.x / 2,
            Center.y - RoomSize.y / 2,
            RoomSize.x,
            RoomSize.y
        );
    }

    // Check room edge tiles (xMax/yMax are exclusive bounds)
    public bool IsEdgeTile(Vector2Int position) {
        if (!Bounds.Contains(position)) {
            return false;
        }

        return position.x == Bounds.xMin ||
                position.x == Bounds.xMax - 1 ||
                position.y == Bounds.yMin ||
                position.y == Bounds.yMax - 1;
    }

    public bool IsWallTile(Vector2Int position) {
        return IsEdgeTile(position) && !IsDoorTile(position);
    }

    public bool IsFloorTile(Vector2Int position) {
        return Bounds.Contains(position) && (!IsEdgeTile(position) || IsDoorTile(position));
    }

    public bool IsDoorTile(Vector2Int position) {
        foreach (Door door in Doors.Values) {
            if (door.TilePositions.Contains(position)) {
                return true;
            }
        }
        return false;
    }
}
