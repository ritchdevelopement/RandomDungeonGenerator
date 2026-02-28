using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoomManager : MonoBehaviour {
    public static RoomManager Instance { get; private set; }

    private readonly HashSet<Room> clearedRooms = new();
    private readonly Dictionary<Room, RoomTrigger> roomTriggers = new();
    private Room playerSpawnRoom;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void SetContext(DungeonGenerationContext ctx) {
        playerSpawnRoom = ctx.playerSpawnRoom;
        if (FogOfWarManager.Instance != null) {
            FogOfWarManager.Instance.RevealRoom(ctx.playerSpawnRoom);
        }
    }

    public void RegisterTrigger(Room room, RoomTrigger trigger) {
        roomTriggers[room] = trigger;
    }

    public void AssignRandomEventsToSiblings(Room activatedRoom) {
        foreach (Room parentRoom in ClearedRoomsIncludingSpawn()) {
            if (!parentRoom.Neighbors.ContainsValue(activatedRoom)) {
                continue;
            }
            foreach (Room sibling in parentRoom.Neighbors.Values) {
                if (sibling != activatedRoom && !clearedRooms.Contains(sibling)) {
                    AssignRandomEvent(sibling);
                }
            }
        }
    }

    private IEnumerable<Room> ClearedRoomsIncludingSpawn() {
        foreach (Room room in clearedRooms) {
            yield return room;
        }
        if (playerSpawnRoom != null) {
            yield return playerSpawnRoom;
        }
    }

    private void AssignRandomEvent(Room room) {
        if (!roomTriggers.TryGetValue(room, out RoomTrigger trigger) || trigger == null) {
            return;
        }
        trigger.SetEventType(RandomEventType());
        FogOfWarManager.Instance.RevealRoom(room);
    }

    private static RoomEventType RandomEventType() {
        RoomEventType[] events = { RoomEventType.Empty, RoomEventType.Cursed, RoomEventType.Bonus };
        return events[Random.Range(0, events.Length)];
    }

    public void AddClearedRoom(Room room) {
        clearedRooms.Add(room);
        roomTriggers.Remove(room);
        ResetSiblingEvents(room);
    }

    private void ResetSiblingEvents(Room clearedRoom) {
        foreach (Room parentRoom in ClearedRoomsIncludingSpawn()) {
            if (!parentRoom.Neighbors.ContainsValue(clearedRoom)) {
                continue;
            }
            foreach (Room sibling in parentRoom.Neighbors.Values) {
                if (sibling != clearedRoom && !clearedRooms.Contains(sibling)) {
                    ResetToNormalEvent(sibling);
                }
            }
        }
    }

    private void ResetToNormalEvent(Room room) {
        if (!roomTriggers.TryGetValue(room, out RoomTrigger trigger) || trigger == null) {
            return;
        }
        trigger.SetEventType(RoomEventType.Normal);
    }

    public bool IsRoomCleared(Room room) {
        return clearedRooms.Contains(room);
    }
}
