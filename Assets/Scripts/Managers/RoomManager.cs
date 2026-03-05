using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoomManager : MonoBehaviour {
    public static RoomManager Instance { get; private set; }

    [SerializeField]
    private RoomTypeWeight[] roomTypeWeights = {
        new RoomTypeWeight { type = RoomEventType.Wave,     weight = 3f },
        new RoomTypeWeight { type = RoomEventType.Survival, weight = 1f },
    };

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
    }

    public void RegisterTrigger(Room room, RoomTrigger trigger) {
        roomTriggers[room] = trigger;
    }

    public void AssignRandomEventsToSiblings(Room activatedRoom) {
        ForEachUnclearedSibling(activatedRoom, AssignRandomEvent);
    }

    public void AddClearedRoom(Room room) {
        clearedRooms.Add(room);
        roomTriggers.Remove(room);
    }

    private void ForEachUnclearedSibling(Room excludedRoom, System.Action<Room> action) {
        foreach (Room parentRoom in ClearedRoomsIncludingSpawn()) {
            if (!parentRoom.Neighbors.ContainsValue(excludedRoom)) {
                continue;
            }
            foreach (Room sibling in parentRoom.Neighbors.Values) {
                if (sibling != excludedRoom && !clearedRooms.Contains(sibling)) {
                    action(sibling);
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

    public void AssignInitialEvents() {
        if (playerSpawnRoom == null) {
            return;
        }

        foreach (Room neighbor in playerSpawnRoom.Neighbors.Values) {
            AssignRandomEvent(neighbor);
        }
    }

    private void AssignRandomEvent(Room room) {
        bool found = roomTriggers.TryGetValue(room, out RoomTrigger trigger);
        Debug.Log($"[RoomManager] AssignRandomEvent: room={room.Center}, found={found}, triggerNull={trigger == null}");
        if (!found || trigger == null) {
            return;
        }

        RoomEventType type = RandomEventType();
        Debug.Log($"[RoomManager] AssignRandomEvent: → {type} to room {room.Center}");
        trigger.SetEventType(type);
    }

    private RoomEventType RandomEventType() {
        float totalWeight = 0f;
        foreach (RoomTypeWeight entry in roomTypeWeights) {
            totalWeight += entry.weight;
        }

        float roll = Random.Range(0f, totalWeight);
        float accumulated = 0f;
        foreach (RoomTypeWeight entry in roomTypeWeights) {
            accumulated += entry.weight;
            if (roll <= accumulated) {
                return entry.type;
            }
        }

        return roomTypeWeights[roomTypeWeights.Length - 1].type;
    }

    public int ClearedRoomCount => clearedRooms.Count;

    public bool IsRoomCleared(Room room) {
        return clearedRooms.Contains(room);
    }
}
