using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour {
    [SerializeField] private float slideSpeed = 5f;
    [SerializeField] private float minPlayerDistanceToClose = 3f;

    private BoxCollider2D mainCollider;
    private SpriteRenderer spriteRenderer;
    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Coroutine slideCoroutine;
    private Transform playerTransform;

    private void Awake() {
        mainCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Door door, Room roomA, Room roomB) {
        closedPosition = transform.position;
        openPosition = CalculateOpenPosition();
        DoorManager.Instance.RegisterDoor(door, this);
    }

    private Vector2 CalculateOpenPosition() {
        Vector2 doorSize = spriteRenderer.size;
        bool isHorizontal = doorSize.x > doorSize.y;
        return isHorizontal
            ? closedPosition + new Vector2(doorSize.x, 0f)
            : closedPosition + new Vector2(0f, doorSize.y);
    }

    public void Open() {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideTo(openPosition));
    }

    public void Close() {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(CloseWhenPlayerClear());
    }

    private IEnumerator CloseWhenPlayerClear() {
        if (playerTransform == null) {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null) playerTransform = playerGO.transform;
        }

        if (playerTransform != null) {
            yield return new WaitUntil(() =>
                Vector2.Distance(playerTransform.position, transform.position) >= minPlayerDistanceToClose
            );
        }

        yield return StartCoroutine(SlideTo(closedPosition));
    }

    private IEnumerator SlideTo(Vector2 target) {
        mainCollider.enabled = false;
        while ((Vector2)transform.position != target) {
            transform.position = Vector2.MoveTowards(transform.position, target, slideSpeed * Time.deltaTime);
            yield return null;
        }
        mainCollider.enabled = target == closedPosition;
    }
}
