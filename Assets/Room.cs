using System.Collections.Generic;
using UnityEngine;

public class Room {
    public Dictionary<Direction, Room> neighbourRooms;
    public Vector2Int roomPos;
    public Vector2Int roomSize;

    public bool doorTop, doorBottom, doorLeft, doorRight;

    public Room(Vector2Int roomSize, Vector2Int roomPos) {
        this.roomSize = roomSize;
        this.roomPos = roomPos;
        neighbourRooms = new Dictionary<Direction, Room>();
    }

    public List<Vector2Int> GetNeighbourPositions() {
        int spacingX = roomSize.x + 1;
        int spacingY = roomSize.y + 1;
        return new List<Vector2Int> {
            new Vector2Int(roomPos.x - spacingX, roomPos.y),
            new Vector2Int(roomPos.x + spacingX, roomPos.y),
            new Vector2Int(roomPos.x, roomPos.y + spacingY),
            new Vector2Int(roomPos.x, roomPos.y - spacingY)
        };
    }

    public void Connect(Room neighborRoom) {
        Direction? direction = GetDirectionTo(neighborRoom);
        if(direction == null) return;

        switch(direction) {
            case Direction.North:
                doorTop = true;
                neighborRoom.doorBottom = true;
                break;
            case Direction.East:
                doorRight = true;
                neighborRoom.doorLeft = true;
                break;
            case Direction.South:
                doorBottom = true;
                neighborRoom.doorTop = true;
                break;
            case Direction.West:
                doorLeft = true;
                neighborRoom.doorRight = true;
                break;
        }

        if(!neighbourRooms.ContainsKey(direction.Value)) {
            neighbourRooms.Add(direction.Value, neighborRoom);
        }
    }

    private Direction? GetDirectionTo(Room other) {
        Vector2Int delta = other.roomPos - roomPos;

        if(delta.y > 0 && delta.x == 0) return Direction.North;
        if(delta.y < 0 && delta.x == 0) return Direction.South;
        if(delta.x > 0 && delta.y == 0) return Direction.East;
        if(delta.x < 0 && delta.y == 0) return Direction.West;

        return null;
    }

}
