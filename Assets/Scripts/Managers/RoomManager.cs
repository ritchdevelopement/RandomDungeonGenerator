using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public static RoomManager Instance { get; private set; }

    private DungeonGenerationContext context;
    private Transform player;
    private Room currentRoom;
    private readonly HashSet<Room> clearedRooms = new();

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
    }

    public void SetContext(DungeonGenerationContext ctx) {
        context = ctx;
        currentRoom = ctx.playerSpawnRoom;
    }

    public void SetPlayer(Transform playerTransform) {
        player = playerTransform;
    }

    public void AddClearedRoom(Room room) {
        clearedRooms.Add(room);
    }

    private void Update() {
        if(player == null) return;

        currentRoom = GetRoomForPosition(player.position);
        if(IsRoomCleared(currentRoom)) return;

        EnemyManager.Instance.OnPlayerEnterRoom(currentRoom);
    }

    public bool IsRoomCleared(Room room) {
        return clearedRooms.Contains(room);
    }

    private Room GetRoomForPosition(Vector2 position) {
        foreach(Room room in context.createdRooms.Values) {
            int halfWidth = room.RoomSize.x / 2;
            int halfHeight = room.RoomSize.y / 2;
            if(position.x >= room.Center.x - halfWidth && position.x <= room.Center.x + halfWidth &&
                position.y >= room.Center.y - halfHeight && position.y <= room.Center.y + halfHeight) {
                return room;
            }
        }
        return null;
    }
}
