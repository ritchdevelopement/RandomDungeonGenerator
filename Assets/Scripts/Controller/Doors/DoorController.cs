using UnityEngine;

public class DoorController : MonoBehaviour {
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Register(Door door) {
        DoorManager.Instance.RegisterDoor(door, this);
    }

    public void Open() {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void Close() {
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
    }
}
