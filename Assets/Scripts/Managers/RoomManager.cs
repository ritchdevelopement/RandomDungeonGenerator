using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoomManager : MonoBehaviour {
    public static RoomManager Instance { get; private set; }

    private readonly HashSet<Room> clearedRooms = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void SetContext(DungeonGenerationContext ctx) {
        if (FogOfWarManager.Instance != null) {
            FogOfWarManager.Instance.RevealRoom(ctx.playerSpawnRoom);
        }
    }

    public void AddClearedRoom(Room room) {
        clearedRooms.Add(room);
    }


    public bool IsRoomCleared(Room room) {
        return clearedRooms.Contains(room);
    }


}
