using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PerkSelectionTrigger : MonoBehaviour {
    private Room room;
    private EncounterMode encounterMode;

    public void Initialize(Room targetRoom, EncounterMode mode) {
        room = targetRoom;
        encounterMode = mode;

        BoxCollider2D triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        PerkManager.Instance.OnPerkSelected(room, encounterMode);
        Destroy(gameObject);
    }
}
