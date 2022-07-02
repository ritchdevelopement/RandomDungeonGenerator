using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject wallTile;

    public void DrawRoom(Room room) {
        GameObject roomGameObject = new GameObject("Room_" + room.roomPos.x + "_" + room.roomPos.y);
        DrawWalls(room, roomGameObject);
    }

    private void DrawWalls(Room room, GameObject roomGameObject) {
        GameObject wallGameObject = new GameObject("Wall");
        for(int i = 0; i <= room.roomSize.x; i++) {
            for(int j = 0; j <= room.roomSize.y; j++) {
                float leftBoundary = -room.roomSize.x / 2 + room.roomPos.x;
                float topBoundary = room.roomSize.y / 2 + room.roomPos.y;
                float rightBoundary = room.roomSize.x / 2 + room.roomPos.x;
                float bottomBoundary = -room.roomSize.y / 2 + room.roomPos.y;

                if(
                    leftBoundary + i == leftBoundary ||
                    topBoundary + j == topBoundary ||
                    leftBoundary + i == rightBoundary ||
                    topBoundary - j == bottomBoundary
                ) {
                    Vector2 tilePosition = new Vector2(leftBoundary + i, topBoundary - j);
                    if(!isDoor(room, tilePosition)) {
                        Instantiate(wallTile, tilePosition, Quaternion.identity).transform.parent = wallGameObject.transform;
                    }
                    wallGameObject.transform.parent = roomGameObject.transform;
                }
            }
        }
    }

    private bool isDoor(Room room, Vector2 tilePosition) {
        Dictionary<string, Room> neighbourRooms = room.neighbourRooms;
        foreach(KeyValuePair<string, Room> neighbour in neighbourRooms) {
            if(neighbour.Key == "N") {
                float bottomBoundary = -room.roomSize.y / 2 + room.roomPos.y;
                Vector2[] door = {
                    new Vector2(room.roomPos.x, bottomBoundary),
                    new Vector2(room.roomPos.x + 1, bottomBoundary),
                    new Vector2(room.roomPos.x - 1, bottomBoundary)
                };
                if(tilePosition.Equals(door[0]) || tilePosition.Equals(door[1]) || tilePosition.Equals(door[2])) {
                    return true;
                }
            } else if(neighbour.Key == "E") {
                float rightBoundary = room.roomSize.x / 2 + room.roomPos.x;
                Vector2[] door = {
                    new Vector2(rightBoundary, room.roomPos.y),
                    new Vector2(rightBoundary, room.roomPos.y + 1),
                    new Vector2(room.roomSize.x / 2 + room.roomPos.x, room.roomPos.y - 1)
                };
                if(tilePosition.Equals(door[0]) || tilePosition.Equals(door[1]) || tilePosition.Equals(door[2])) {
                    return true;
                }
            } else if(neighbour.Key == "S") {
                float topBoundary = room.roomSize.y / 2 + room.roomPos.y;
                Vector2[] door = {
                    new Vector2(room.roomPos.x, topBoundary),
                    new Vector2(room.roomPos.x + 1, topBoundary),
                    new Vector2(room.roomPos.x - 1, topBoundary)
                };
                if(tilePosition.Equals(door[0]) || tilePosition.Equals(door[1]) || tilePosition.Equals(door[2])) {
                    return true;
                }
            } else if(neighbour.Key == "W") {
                float leftBoundary = -room.roomSize.x / 2 + room.roomPos.x;
                Vector2[] door = {
                    new Vector2(leftBoundary, room.roomPos.y),
                    new Vector2(leftBoundary, room.roomPos.y + 1),
                    new Vector2(leftBoundary, room.roomPos.y - 1)
                };
                if(tilePosition.Equals(door[0]) || tilePosition.Equals(door[1]) || tilePosition.Equals(door[2])) {
                    return true;
                }
            }
        }
        return false;
    }
}
