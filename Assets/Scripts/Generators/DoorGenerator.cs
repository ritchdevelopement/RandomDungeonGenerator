using System.Collections.Generic;
using UnityEngine;

public class DoorGenerator : DungeonTaskBase {
    public override void Execute() {
        GenerateDoors();
    }

    public void GenerateDoors() {
        foreach(Room room in context.createdRooms.Values) {
            foreach(Vector2Int pos in room.GetNeighbourPositions()) {
                if(context.createdRooms.TryGetValue(pos, out Room neighbour)) {
                    if(room.Neighbors.ContainsValue(neighbour)) {
                        continue;
                    }

                    Direction dir = room.GetDirectionTo(neighbour);
                    List<Vector2Int> doorTiles = GetDoorTilePositions(room, dir);

                    Door door = new Door(doorTiles);
                    room.Connect(neighbour, door, dir);
                    context.createdDoors.Add(door);
                }
            }
        }
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
}
