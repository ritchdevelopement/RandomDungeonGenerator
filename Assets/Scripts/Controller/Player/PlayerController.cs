using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rigidbody2d;
    private Vector2 moveInput;

    private void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        HandleInput();
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(1);
        }
    }

    private void HandleInput() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;
    }

    private void HandleMovement() {
        rigidbody2d.linearVelocity = moveInput * moveSpeed;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
        rigidbody2d.linearVelocity = Vector2.zero;
    }
}
