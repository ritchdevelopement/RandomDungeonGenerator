using System.Collections.Generic;
using UnityEngine;

public class DoorGenerator : DungeonTaskBase {
    [SerializeField] private int doorWidth = 4;
    private const int DoorDepth = 2;

    public override void Execute() {
        GenerateDoors();
    }

    private void GenerateDoors() {
        foreach ((Room roomA, Room roomB, Direction dir) in context.adjacencies) {
            if (roomA.Neighbors.ContainsValue(roomB)) {
                continue;
            }

            List<Vector2Int> doorTiles = GetDoorTilePositions(roomA, roomB, dir);
            Door door = new Door(doorTiles);
            door.SetConnectedRooms(roomA, roomB);
            roomA.Connect(roomB, door, dir);
            context.createdDoors.Add(door);
        }
    }

    private List<Vector2Int> GetDoorTilePositions(Room room, Room neighbor, Direction direction) {
        List<Vector2Int> tiles = new();
        int halfWidth = EffectiveDoorWidth(room, neighbor, direction) / 2;
        Vector2Int center = room.Center;

        Vector2Int basePos = direction switch {
            Direction.North => new Vector2Int(center.x, center.y + room.RoomSize.y / 2),
            Direction.South => new Vector2Int(center.x, center.y - room.RoomSize.y / 2),
            Direction.East => new Vector2Int(center.x + room.RoomSize.x / 2, center.y),
            Direction.West => new Vector2Int(center.x - room.RoomSize.x / 2, center.y),
            _ => center
        };

        for (int offset = -halfWidth; offset <= halfWidth; offset++) {
            for (int depth = 0; depth < DoorDepth; depth++) {
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

    private int EffectiveDoorWidth(Room room, Room neighbor, Direction direction) {
        bool isNorthSouth = direction == Direction.North || direction == Direction.South;
        int perpendicularA = isNorthSouth ? room.RoomSize.x : room.RoomSize.y;
        int perpendicularB = isNorthSouth ? neighbor.RoomSize.x : neighbor.RoomSize.y;
        int maxAllowed = Mathf.Min(perpendicularA, perpendicularB) - 2;
        return Mathf.Clamp(doorWidth, 1, maxAllowed);
    }
}
