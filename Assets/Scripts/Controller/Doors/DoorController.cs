using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DoorController : MonoBehaviour {
    private BoxCollider2D mainCollider;
    private Animator[] allAnimators;

    private static readonly int IsOpenParam = Animator.StringToHash("IsOpen");

    private void Awake() {
        mainCollider = GetComponent<BoxCollider2D>();
    }

    public void Initialize(Door door, Room roomA, Room roomB) {
        mainCollider = GetComponent<BoxCollider2D>();
        allAnimators = GetComponentsInChildren<Animator>(true);
        DoorManager.Instance.RegisterDoor(door, this);
    }

    public void Open() {
        mainCollider.enabled = false;
        SetIsOpen(true);
    }

    public void Close() {
        SetIsOpen(false);
        mainCollider.enabled = true;
    }

    private void SetIsOpen(bool isOpen) {
        if (allAnimators == null) {
            return;
        }

        foreach (Animator animator in allAnimators) {
            animator.SetBool(IsOpenParam, isOpen);
        }
    }
}
