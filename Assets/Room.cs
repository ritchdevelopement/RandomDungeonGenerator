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
        List<Vector2Int> neighbourPositions = new List<Vector2Int>
        {
            new Vector2Int(roomPos.x - roomSize.x - 1, roomPos.y),
            new Vector2Int(roomPos.x + roomSize.x + 1, roomPos.y),
            new Vector2Int(roomPos.x, roomPos.y + roomSize.y + 1),
            new Vector2Int(roomPos.x, roomPos.y - roomSize.y - 1)
        };
        return neighbourPositions;
    }

    public void Connect(Room neighborRoom) {
        Direction direction = Direction.North;

        if(neighborRoom.roomPos.y < roomPos.y) {
            direction = Direction.North;
        } else if(neighborRoom.roomPos.x > roomPos.x) {
            direction = Direction.East;
        } else if(neighborRoom.roomPos.y > roomPos.y) {
            direction = Direction.South;
        } else if(neighborRoom.roomPos.x < roomPos.x) {
            direction = Direction.West;
        }

        if(!neighbourRooms.ContainsKey(direction)) {
            neighbourRooms.Add(direction, neighborRoom);
        }
    }
}
