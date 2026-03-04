using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable {
    private const string AnimatorWalkingParam = "IsWalking";

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int health = 5;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private float separationStrength = 1.5f;

    private Transform playerTransform;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public event System.Action OnDeath;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initialize(Transform player, EnemyData data) {
        playerTransform = player;
        health = data.health;
        moveSpeed = data.moveSpeed;
        contactDamage = data.contactDamage;
    }

    private void FixedUpdate() {
        MoveTowardsPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(contactDamage);
        }
    }

    private void MoveTowardsPlayer() {
        if (playerTransform == null) {
            SetWalking(false);
            return;
        }

        Vector2 toPlayer = ((Vector2) playerTransform.position - (Vector2) transform.position).normalized;
        Vector2 separation = CalculateSeparationForce();
        Vector2 moveDirection = (toPlayer + separation).normalized;

        rigidBody.linearVelocity = moveDirection * moveSpeed;
        SetWalking(true);
        FaceMovementDirection();
    }

    // Pushes this enemy away from nearby enemies to prevent stacking
    private Vector2 CalculateSeparationForce() {
        Vector2 totalSeparation = Vector2.zero;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (Collider2D neighbor in neighbors) {
            if (neighbor.gameObject == gameObject)
                continue;
            if (!neighbor.TryGetComponent(out EnemyController _))
                continue;

            Vector2 awayFromNeighbor = (Vector2) transform.position - (Vector2) neighbor.transform.position;
            float distance = awayFromNeighbor.magnitude;

            if (distance > 0f) {
                totalSeparation += awayFromNeighbor.normalized * (separationStrength / distance);
            }
        }

        return totalSeparation;
    }

    private void SetWalking(bool isWalking) {
        if (animator != null) {
            animator.SetBool(AnimatorWalkingParam, isWalking);
        }
    }

    private void FaceMovementDirection() {
        spriteRenderer.flipX = playerTransform.position.x < transform.position.x;
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            OnDeath?.Invoke();
            Destroy(gameObject);
        } else {
            StartCoroutine(FlashHitColor());
        }
    }

    private IEnumerator FlashHitColor() {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
