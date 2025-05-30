using System.Collections.Generic;
using UnityEngine;

public class Room {
    public Dictionary<Direction, Room> Neighbors { get; } = new();
    public Dictionary<Direction, Door> Doors { get; } = new();
    public Vector2Int Center { get; }
    public Vector2Int RoomSize { get; }
    public float DistanceFromCenter { get; private set; }

    public Room(Vector2Int roomSize, Vector2Int center) {
        RoomSize = roomSize;
        Center = center;
        DistanceFromCenter = Vector2Int.Distance(center, Vector2Int.zero);
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

    public void Connect(Room neighbor) {
        Direction dir = GetDirectionTo(neighbor);
        Direction opposite = dir.Opposite();

        Neighbors[dir] = neighbor;
        neighbor.Neighbors[opposite] = this;

        var door = new Door(GetDoorTilePositions(this, dir));
        Doors[dir] = door;
        neighbor.Doors[opposite] = door;
    }

    private Direction GetDirectionTo(Room neighbor) {
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

    private List<Vector2Int> GetDoorTilePositions(Room room, Direction direction, int doorWidth = 3, int doorDepth = 2) {
        List<Vector2Int> tiles = new();
        int halfWidth = doorWidth / 2;
        Vector2Int center = room.Center;

        Vector2Int basePos = direction switch {
            Direction.North => new Vector2Int(center.x, center.y + room.RoomSize.y / 2),
            Direction.South => new Vector2Int(center.x, center.y - room.RoomSize.y / 2),
            Direction.East => new Vector2Int(center.x + room.RoomSize.x / 2, center.y),
            Direction.West => new Vector2Int(center.x - room.RoomSize.x / 2, center.y),
            _ => center
        };

        for(int offset = -halfWidth; offset <= halfWidth; offset++) {
            for(int depth = 0; depth < doorDepth; depth++) {
                Vector2Int tile = direction switch {
                    Direction.North => new Vector2Int(basePos.x + offset, basePos.y + depth),
                    Direction.South => new Vector2Int(basePos.x + offset, basePos.y - depth),
                    Direction.East => new Vector2Int(basePos.x + depth, basePos.y + offset),
                    Direction.West => new Vector2Int(basePos.x - depth, basePos.y + offset),
                    _ => basePos
                };
                tiles.Add(tile);
            }
            
        }

        return tiles;
    }

    public bool HasDoor(Direction direction) => Doors.ContainsKey(direction);
}
