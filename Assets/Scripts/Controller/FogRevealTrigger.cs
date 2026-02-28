using UnityEngine;

public class FogRevealTrigger : MonoBehaviour {
    private Room room;

    public void Initialize(Room room) {
        this.room = room;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        FogOfWarManager.Instance.RevealRoom(room);
    }
}
