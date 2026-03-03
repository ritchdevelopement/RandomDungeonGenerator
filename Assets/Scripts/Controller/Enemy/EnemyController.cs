using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable {
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int health = 5;
    [SerializeField] private float invulnerabilityTime = 1f;

    private Transform playerTransform;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;

    public event System.Action OnDeath;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Transform player) {
        playerTransform = player;
    }

    private void FixedUpdate() {
        MoveTowardsPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(1);
        }
    }

    private void MoveTowardsPlayer() {
        if (playerTransform == null) {
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rigidBody.linearVelocity = direction * moveSpeed;
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            OnDeath?.Invoke();
            Destroy(gameObject);
        } else {
            StartInvulnerability();
        }
    }

    private void StartInvulnerability() {
        InvokeRepeating(nameof(FlashSprite), 0f, 0.1f);
        Invoke(nameof(EndInvulnerability), invulnerabilityTime);
    }

    private void EndInvulnerability() {
        CancelInvoke(nameof(FlashSprite));
        if (spriteRenderer != null) {
            spriteRenderer.color = Color.white;
        }
    }

    private void FlashSprite() {
        spriteRenderer.color = spriteRenderer.color == Color.white ? Color.gray : Color.white;
    }
}
