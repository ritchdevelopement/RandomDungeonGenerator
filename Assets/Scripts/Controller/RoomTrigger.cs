using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class RoomTrigger : MonoBehaviour {
    private Room room;
    private RoomEventType eventType = RoomEventType.Normal;
    private EncounterMode encounterMode = EncounterMode.Wave;
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

    public void SetEncounterMode(EncounterMode mode) {
        encounterMode = mode;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        ActivateRoom();
    }

    private void ActivateRoom() {
        RoomManager.Instance.AssignRandomEventsToSiblings(room);

        if (eventType == RoomEventType.Bonus) {
            PerkManager.Instance.ShowPerkSelection(room, encounterMode);
        } else if (eventType != RoomEventType.Empty) {
            EnemyManager.Instance.StartRoomEncounter(room, encounterMode);
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
