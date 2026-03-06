using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour {
    private Room room;
    private RoomEventType eventType = RoomEventType.Wave;
    private bool hasBeenActivated;

    public void Initialize(Room room, bool preActivated = false) {
        this.room = room;
        hasBeenActivated = preActivated;
    }

    public void SetEventType(RoomEventType type) {
        eventType = type;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        RoomManager.Instance.NotifyRoomEntered(room);

        if (!hasBeenActivated) {
            hasBeenActivated = true;
            Debug.Log($"[RoomTrigger] Room {room.Center} → {eventType}");
            RoomManager.Instance.AssignRandomEventsToSiblings(room);
            EnemyManager.Instance.StartRoomEncounter(room, eventType);
        }
    }
}
