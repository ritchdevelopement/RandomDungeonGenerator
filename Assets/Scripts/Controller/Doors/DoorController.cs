using UnityEngine;

public class DoorController : MonoBehaviour {
    public float teleportDistance = 2f;
    private BoxCollider2D mainCollider;
    private SpriteRenderer spriteRenderer;
    private Room roomA;
    private Room roomB;

    private void Awake() {
        mainCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Door door, Room roomA, Room roomB) {
        this.roomA = roomA;
        this.roomB = roomB;
        DoorManager.Instance.RegisterDoor(door, this);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.TryGetComponent<PlayerController>(out var player))
            return;

        Vector2 targetPosition = DetermineTargetPosition(other.transform.position);
        player.SetPosition(targetPosition);
    }

    private Vector2 DetermineTargetPosition(Vector2 playerPosition) {
        Vector2 doorCenter = transform.position;
        Vector2 doorSize = spriteRenderer.size;
        bool isHorizontal = doorSize.x > doorSize.y;

        Vector2 targetPosition;

        if (isHorizontal) {
            if (playerPosition.y > doorCenter.y) {
                targetPosition = new Vector2(doorCenter.x, doorCenter.y - teleportDistance);
            } else {
                targetPosition = new Vector2(doorCenter.x, doorCenter.y + teleportDistance);
            }
        } else {
            if (playerPosition.x < doorCenter.x) {
                targetPosition = new Vector2(doorCenter.x + teleportDistance, doorCenter.y);
            } else {
                targetPosition = new Vector2(doorCenter.x - teleportDistance, doorCenter.y);
            }
        }

        return targetPosition;
    }

    public void Open() {
        mainCollider.isTrigger = true;
        spriteRenderer.color = Color.green;
    }

    public void Close() {
        mainCollider.isTrigger = false;
        spriteRenderer.color = Color.red;
    }
}
