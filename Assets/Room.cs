using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public Dictionary<string, Room> neighbourRooms;

    public Vector2Int roomPos;
    public Vector2Int roomSize;

    public bool doorTop, doorBottom, doorLeft, doorRight;

    public Room(Vector2Int roomSize, Vector2Int roomPos) {
        this.roomSize = roomSize;
        this.roomPos = roomPos;
        neighbourRooms = new Dictionary<string, Room>();
    }

    public List<Vector2Int> GetNeighbourPositions() {
        List<Vector2Int> neighbourPositions = new List<Vector2Int>();
        neighbourPositions.Add(new Vector2Int(roomPos.x - roomSize.x - 1, roomPos.y));
        neighbourPositions.Add(new Vector2Int(roomPos.x + roomSize.x + 1, roomPos.y));
        neighbourPositions.Add(new Vector2Int(roomPos.x, roomPos.y + roomSize.y + 1));
        neighbourPositions.Add(new Vector2Int(roomPos.x, roomPos.y - roomSize.y - 1));
        return neighbourPositions;
    }

    public void Connect(Room neighborRoom) {
        string direction = "";
        if(neighborRoom.roomPos.y < roomPos.y) {
            direction = "N";
        }
        if(neighborRoom.roomPos.x > roomPos.x) {
            direction = "E";
        }
        if(neighborRoom.roomPos.y > roomPos.y) {
            direction = "S";
        }
        if(neighborRoom.roomPos.x < roomPos.x) {
            direction = "W";
        }
        neighbourRooms.Add(direction, neighborRoom);
    }
}
