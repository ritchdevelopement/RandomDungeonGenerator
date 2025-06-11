using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rigidbody2d;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool facingRight = false;

    private void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        HandleInput();
        HandleAnimation();
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


        if(horizontal == 0) {
            return;

        }

        bool shouldFaceRight = horizontal > 0;
        if (shouldFaceRight != facingRight) {
            facingRight = shouldFaceRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    private void HandleAnimation() {
        bool isMoving = moveInput.magnitude > 0;
        animator.SetBool("IsMoving", isMoving);
    }

    private void HandleMovement() {
        rigidbody2d.linearVelocity = moveInput * moveSpeed;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
        rigidbody2d.linearVelocity = Vector2.zero;
    }
}
