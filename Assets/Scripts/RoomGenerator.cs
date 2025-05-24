using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator {
    private GameObject allRoomsParent = new GameObject("Rooms");
    private GameObject wallTile;

    public RoomGenerator(GameObject wallTile) {
        this.wallTile = wallTile ?? throw new MissingReferenceException("WallTile must not be null.");
    }

    public void DrawRoom(Room room) {
        GameObject roomGameObject = new GameObject("Room_" + room.roomPos.x + "_" + room.roomPos.y);
        roomGameObject.transform.parent = allRoomsParent.transform;
        DrawWalls(room, roomGameObject);
    }

    private void DrawWalls(Room room, GameObject roomGameObject) {
        GameObject wallGameObject = new GameObject("Wall");

        float left = -room.roomSize.x / 2f + room.roomPos.x;
        float right = room.roomSize.x / 2f + room.roomPos.x;
        float top = room.roomSize.y / 2f + room.roomPos.y;
        float bottom = -room.roomSize.y / 2f + room.roomPos.y;

        for(int i = 0; i <= room.roomSize.x; i++) {
            for(int j = 0; j <= room.roomSize.y; j++) {
                Vector2 tilePos = new Vector2(left + i, top - j);
                Vector2Int tile = Vector2Int.RoundToInt(tilePos);

                bool isEdgeTile =
                    Mathf.Approximately(tilePos.x, left) ||
                    Mathf.Approximately(tilePos.x, right) ||
                    Mathf.Approximately(tilePos.y, top) ||
                    Mathf.Approximately(tilePos.y, bottom);

                if(isEdgeTile && !IsDoorTile(room, tile)) {
                    Object.Instantiate(wallTile, tilePos, Quaternion.identity).transform.parent = wallGameObject.transform;
                }
            }
        }

        wallGameObject.transform.parent = roomGameObject.transform;
    }

    private bool IsDoorTile(Room room, Vector2Int tile) {
        int x = room.roomPos.x;
        int y = room.roomPos.y;
        int halfWidth = room.roomSize.x / 2;
        int halfHeight = room.roomSize.y / 2;

        var doorOffsets = new Dictionary<Direction, (bool isOpen, Vector2Int center)> {
            { Direction.North, (room.doorTop, new Vector2Int(x, y + halfHeight)) },
            { Direction.South, (room.doorBottom, new Vector2Int(x, y - halfHeight)) },
            { Direction.West, (room.doorLeft, new Vector2Int(x - halfWidth, y)) },
            { Direction.East, (room.doorRight, new Vector2Int(x + halfWidth, y)) }
        };

        foreach((Direction dir, (bool isOpen, Vector2Int center)) in doorOffsets) {
            if(!isOpen) {
                continue;
            }

            // Check if the tile is within the 3x3 area around the door center
            for(int dx = -1; dx <= 1; dx++) {
                for(int dy = -1; dy <= 1; dy++) {
                    if(tile == center + new Vector2Int(dx, dy)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
