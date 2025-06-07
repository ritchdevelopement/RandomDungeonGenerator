using UnityEngine;

public class DoorController : MonoBehaviour {
    private BoxCollider2D mainCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Teleport Triggers")]
    private BoxCollider2D triggerA;
    private BoxCollider2D triggerB;

    private void Awake() {
        mainCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Door door) {
        CreateTriggerZones();
        DoorManager.Instance.RegisterDoor(door, this);
    }

    private void CreateTriggerZones() {
        GameObject triggerGameObjectA = new GameObject("TriggerA");
        triggerGameObjectA.transform.SetParent(transform);
        triggerA = triggerGameObjectA.AddComponent<BoxCollider2D>();
        triggerA.isTrigger = true;

        GameObject triggerGameObjectB = new GameObject("TriggerB");
        triggerGameObjectB.transform.SetParent(transform);
        triggerB = triggerGameObjectB.AddComponent<BoxCollider2D>();
        triggerB.isTrigger = true;

        SetupTriggerSizes();
    }

    private void SetupTriggerSizes() {
        Vector2 doorSize = spriteRenderer.size;
        bool isHorizontal = doorSize.x > doorSize.y;

        if (isHorizontal) {
            SetupHorizontalTriggers(doorSize);
        } else {
            SetupVerticalTriggers(doorSize);
        }
    }

    private void SetupHorizontalTriggers(Vector2 doorSize) {
        float doorHalfHeight = doorSize.y * 0.5f;
        float quarterHeight = doorHalfHeight * 0.5f;

        Vector2 triggerSize = new Vector2(doorSize.x, doorHalfHeight);

        triggerA.size = triggerSize;
        triggerA.transform.localPosition = new Vector3(0, quarterHeight);

        triggerB.size = triggerSize;
        triggerB.transform.localPosition = new Vector3(0, -quarterHeight);
    }

    private void SetupVerticalTriggers(Vector2 doorSize) {
        float doorHalfWidth = doorSize.x * 0.5f;
        float quarterWidth = doorHalfWidth * 0.5f;

        Vector2 triggerSize = new Vector2(doorHalfWidth, doorSize.y);

        triggerA.size = triggerSize;
        triggerA.transform.localPosition = new Vector3(-quarterWidth, 0);

        triggerB.size = triggerSize;
        triggerB.transform.localPosition = new Vector3(quarterWidth, 0);
    }

    public void Open() {
        mainCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void Close() {
        mainCollider.enabled = true;
        spriteRenderer.enabled = true;
    }
}
