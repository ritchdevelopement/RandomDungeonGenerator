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
        return new List<Vector2Int> {
            new Vector2Int(roomPos.x - roomSize.x - 1, roomPos.y), // West
            new Vector2Int(roomPos.x + roomSize.x + 1, roomPos.y), // East
            new Vector2Int(roomPos.x, roomPos.y + roomSize.y + 1), // North
            new Vector2Int(roomPos.x, roomPos.y - roomSize.y - 1) // South
        };
    }

    public void Connect(Room neighborRoom) {
        Direction direction = Direction.North;

        if(neighborRoom.roomPos.y < roomPos.y) {
            direction = Direction.South;
            doorBottom = true;
            neighborRoom.doorTop = true;
        } else if(neighborRoom.roomPos.x > roomPos.x) {
            direction = Direction.East;
            doorRight = true;
            neighborRoom.doorLeft = true;
        } else if(neighborRoom.roomPos.y > roomPos.y) {
            direction = Direction.North;
            doorTop = true;
            neighborRoom.doorBottom = true;
        } else if(neighborRoom.roomPos.x < roomPos.x) {
            direction = Direction.West;
            doorLeft = true;
            neighborRoom.doorRight = true;
        }

        if(!neighbourRooms.ContainsKey(direction)) {
            neighbourRooms.Add(direction, neighborRoom);
        }
    }
}
