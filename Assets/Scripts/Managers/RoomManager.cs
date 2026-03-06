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

    public static event System.Action<Room> OnDungeonLoaded;
    public static event System.Action<Room> OnRoomEntered;

    public void SetContext(DungeonGenerationContext ctx) {
        playerSpawnRoom = ctx.playerSpawnRoom;
        if (Application.isPlaying && playerSpawnRoom != null) {
            OnDungeonLoaded?.Invoke(playerSpawnRoom);
        }
    }

    public void NotifyRoomEntered(Room room) {
        OnRoomEntered?.Invoke(room);
    }

    public void RegisterTrigger(Room room, RoomTrigger trigger) {
        roomTriggers[room] = trigger;
    }

    public void AssignRandomEventsToSiblings(Room activatedRoom) {
        foreach (Room sibling in UnclearedSiblings(activatedRoom)) {
            AssignRandomEvent(sibling);
        }
    }

    public void AddClearedRoom(Room room) {
        clearedRooms.Add(room);
        roomTriggers.Remove(room);
    }

    private IEnumerable<Room> UnclearedSiblings(Room excludedRoom) {
        foreach (Room parentRoom in ClearedRoomsIncludingSpawn()) {
            if (!parentRoom.Neighbors.ContainsValue(excludedRoom)) {
                continue;
            }
            foreach (Room sibling in parentRoom.Neighbors.Values) {
                if (sibling != excludedRoom && !clearedRooms.Contains(sibling)) {
                    yield return sibling;
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
        if (!roomTriggers.TryGetValue(room, out RoomTrigger trigger) || trigger == null) {
            return;
        }

        trigger.SetEventType(PickRandomEventType());
    }

    private RoomEventType PickRandomEventType() {
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
