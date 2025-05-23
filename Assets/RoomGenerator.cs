using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
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

                Vector2 tilePos = new Vector2(leftBoundary + i, topBoundary - j);

                bool isEdgeTile =
                    tilePos.x == leftBoundary ||
                    tilePos.x == rightBoundary ||
                    tilePos.y == topBoundary ||
                    tilePos.y == bottomBoundary;

                if(isEdgeTile && !isDoor(room, tilePos)) {
                    Instantiate(wallTile, tilePos, Quaternion.identity).transform.parent = wallGameObject.transform;
                }
            }
        }
        wallGameObject.transform.parent = roomGameObject.transform;
    }

    private bool isDoor(Room room, Vector2 tilePosition) {
        Vector2Int tile = Vector2Int.RoundToInt(tilePosition);
        Dictionary<Direction, Room> neighbourRooms = room.neighbourRooms;

        foreach(KeyValuePair<Direction, Room> neighbour in neighbourRooms) {
            switch(neighbour.Key) {
                case Direction.North: {
                    int y = room.roomPos.y - room.roomSize.y / 2;
                    Vector2Int[] doors = {
                        new(room.roomPos.x, y),
                        new(room.roomPos.x + 1, y),
                        new(room.roomPos.x - 1, y)
                    };
                    if(System.Array.Exists(doors, d => d == tile))
                        return true;
                    break;
                }
                case Direction.East: {
                    int x = room.roomPos.x + room.roomSize.x / 2;
                    Vector2Int[] doors = {
                        new(x, room.roomPos.y),
                        new(x, room.roomPos.y + 1),
                        new(x, room.roomPos.y - 1)
                    };
                    if(System.Array.Exists(doors, d => d == tile))
                        return true;
                    break;
                }
                case Direction.South: {
                    int y = room.roomPos.y + room.roomSize.y / 2;
                    Vector2Int[] doors = {
                        new(room.roomPos.x, y),
                        new(room.roomPos.x + 1, y),
                        new(room.roomPos.x - 1, y)
                    };
                    if(System.Array.Exists(doors, d => d == tile))
                        return true;
                    break;
                }
                case Direction.West: {
                    int x = room.roomPos.x - room.roomSize.x / 2;
                    Vector2Int[] doors = {
                        new(x, room.roomPos.y),
                        new(x, room.roomPos.y + 1),
                        new(x, room.roomPos.y - 1)
                    };
                    if(System.Array.Exists(doors, d => d == tile))
                        return true;
                    break;
                }
            }
        }
        return false;
    }
}
