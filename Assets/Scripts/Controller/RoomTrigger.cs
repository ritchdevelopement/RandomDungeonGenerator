using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour {
    private Room room;

    public void Initialize(Room room) {
        this.room = room;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        ActivateRoom();
    }

    private void ActivateRoom() {
        EnemyManager.Instance.StartRoomEncounter(room);
        Destroy(gameObject);
    }
}
