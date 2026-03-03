using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");

    private Rigidbody2D rigidbody2d;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private DashGhostTrail dashGhostTrail;
    private bool isFacingRight = true;
    private bool isDashing = false;
    private float dashCooldownRemaining = 0f;

    public static float DashCooldownFraction { get; private set; }

    private void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        dashGhostTrail = GetComponent<DashGhostTrail>();
    }

    private void Update() {
        HandleInput();
        HandleAnimation();
        TickDashCooldown();
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

        if (horizontal != 0) {
            isFacingRight = horizontal > 0;
            spriteRenderer.flipX = !isFacingRight;
        }

        if (CanDash()) {
            StartCoroutine(PerformDash());
        }
    }

    private bool CanDash() {
        return Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownRemaining <= 0f;
    }

    private void HandleAnimation() {
        bool isMoving = moveInput.magnitude > 0;
        animator.SetBool(IsMovingParam, isMoving);
    }

    private void HandleMovement() {
        if (isDashing) {
            return;
        }

        rigidbody2d.linearVelocity = moveInput * moveSpeed;
    }

    private void TickDashCooldown() {
        if (dashCooldownRemaining > 0f) {
            dashCooldownRemaining -= Time.deltaTime;
        }

        DashCooldownFraction = dashCooldown > 0f ? dashCooldownRemaining / dashCooldown : 0f;
    }

    private IEnumerator PerformDash() {
        isDashing = true;
        dashCooldownRemaining = dashCooldown;
        dashGhostTrail?.StartTrail();

        Vector2 dashDirection = GetDashDirection();
        float elapsed = 0f;

        while (elapsed < dashDuration) {
            rigidbody2d.linearVelocity = dashDirection * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        dashGhostTrail?.StopTrail();
        isDashing = false;
    }

    private Vector2 GetDashDirection() {
        if (moveInput.magnitude > 0) {
            return moveInput;
        }

        return isFacingRight ? Vector2.right : Vector2.left;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
        rigidbody2d.linearVelocity = Vector2.zero;
    }
}
