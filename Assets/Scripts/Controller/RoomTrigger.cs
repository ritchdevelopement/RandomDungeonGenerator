using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class RoomTrigger : MonoBehaviour {
    private Room room;
    private RoomEventType eventType = RoomEventType.Normal;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Room room) {
        this.room = room;
    }

    public void SetEventType(RoomEventType type) {
        eventType = type;
        spriteRenderer.color = EventColor(type);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        ActivateRoom();
    }

    private void ActivateRoom() {
        if (eventType != RoomEventType.Empty) {
            EnemyManager.Instance.StartRoomEncounter(room);
        }
        Destroy(gameObject);
    }

    private static Color EventColor(RoomEventType type) => type switch {
        RoomEventType.Empty => Color.gray,
        RoomEventType.Cursed => Color.red,
        RoomEventType.Bonus => Color.yellow,
        _ => Color.white,
    };
}
