using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DoorManager : MonoBehaviour {
    public static DoorManager Instance { get; private set; }

    private readonly Dictionary<Door, DoorController> doorLookup = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void RegisterDoor(Door model, DoorController doorController) {
        if(doorLookup.ContainsKey(model)) {
            return;
        }

        doorLookup.Add(model, doorController);
    }

    public void OpenRoomDoors(Room room) {
        foreach(Door door in room.Doors.Values) {
            if(doorLookup.TryGetValue(door, out DoorController doorController)) {
                doorController.Open();
            }
        }
    }

    public void CloseRoomDoors(Room room) {
        foreach (Door door in room.Doors.Values) {
            if (doorLookup.TryGetValue(door, out DoorController doorController)) {
                doorController.Close();
            }
        }
    }
}
