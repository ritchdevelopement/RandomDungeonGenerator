using UnityEngine;

public class PerkManager : MonoBehaviour {
    public static PerkManager Instance { get; private set; }

    [SerializeField] private Sprite perkTriggerSprite;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ShowPerkSelection(Room room, EncounterMode encounterMode) {
        GameObject triggerObject = CreatePerkTriggerObject(room);
        PerkSelectionTrigger perkTrigger = triggerObject.AddComponent<PerkSelectionTrigger>();
        perkTrigger.Initialize(room, encounterMode);
    }

    public void OnPerkSelected(Room room, EncounterMode encounterMode) {
        EnemyManager.Instance.StartRoomEncounter(room, encounterMode);
    }

    private GameObject CreatePerkTriggerObject(Room room) {
        GameObject triggerObject = new("PerkSelectionTrigger");
        triggerObject.transform.position = new Vector3(room.Center.x, room.Center.y, 0f);

        SpriteRenderer spriteRenderer = triggerObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = perkTriggerSprite;
        spriteRenderer.color = Color.yellow;

        triggerObject.AddComponent<BoxCollider2D>();

        return triggerObject;
    }
}
