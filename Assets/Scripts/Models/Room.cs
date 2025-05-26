using System.Collections.Generic;
using UnityEngine;

public class Room {
    public Dictionary<Direction, Room> NeighbourRooms { get; } = new();

    public Vector2Int RoomPos { get; private set; }
    public Vector2Int RoomSize { get; private set; }
    public float DistanceFromCenter { get; private set; }
    public bool DoorTop { get; private set; }
    public bool DoorBottom { get; private set; }
    public bool DoorLeft { get; private set; }
    public bool DoorRight { get; private set; }

    public Room(Vector2Int roomSize, Vector2Int roomPos) {
        RoomSize = roomSize;
        RoomPos = roomPos;
        DistanceFromCenter = Vector2Int.Distance(roomPos, Vector2Int.zero);
    }

    public List<Vector2Int> GetNeighbourPositions() {
        int spacingX = RoomSize.x + 1;
        int spacingY = RoomSize.y + 1;
        return new List<Vector2Int> {
            new Vector2Int(RoomPos.x - spacingX, RoomPos.y),
            new Vector2Int(RoomPos.x + spacingX, RoomPos.y),
            new Vector2Int(RoomPos.x, RoomPos.y + spacingY),
            new Vector2Int(RoomPos.x, RoomPos.y - spacingY)
        };
    }

    public void Connect(Room neighborRoom) {
        Direction? direction = GetDirectionTo(neighborRoom);
        if(direction == null) {
            return;
        }

        switch(direction) {
            case Direction.North: {
                DoorTop = true;
                neighborRoom.DoorBottom = true;
                break;
            }
            case Direction.East: {
                DoorRight = true;
                neighborRoom.DoorLeft = true;
                break;
            }
            case Direction.South: {
                DoorBottom = true;
                neighborRoom.DoorTop = true;
                break;
            }
            case Direction.West: {
                DoorLeft = true;
                neighborRoom.DoorRight = true;
                break;
            }
        }

        if(!NeighbourRooms.ContainsKey(direction.Value)) {
            NeighbourRooms.Add(direction.Value, neighborRoom);
        }
    }

    private Direction? GetDirectionTo(Room other) {
        Vector2Int delta = other.RoomPos - RoomPos;

        if(delta.y > 0 && delta.x == 0) {
            return Direction.North;
        }
        if(delta.y < 0 && delta.x == 0) {
            return Direction.South;
        }
        if(delta.x > 0 && delta.y == 0) {
            return Direction.East;
        }
        if(delta.x < 0 && delta.y == 0) {
            return Direction.West;
        }

        return null;
    }
}
