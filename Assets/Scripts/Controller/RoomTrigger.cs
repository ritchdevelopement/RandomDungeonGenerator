using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour {
    private Room room;
    private RoomEventType eventType = RoomEventType.Wave;

    public void Initialize(Room room) {
        this.room = room;
    }

    public void SetEventType(RoomEventType type) {
        eventType = type;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        ActivateRoom();
    }

    private void ActivateRoom() {
        Debug.Log($"[RoomTrigger] Room {room.Center} → {eventType}");
        RoomManager.Instance.AssignRandomEventsToSiblings(room);
        EnemyManager.Instance.StartRoomEncounter(room, eventType);
        Destroy(gameObject);
    }
}
