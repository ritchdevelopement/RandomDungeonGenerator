using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable {
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private int health = 5;

    [SerializeField]
    private float invulnerabilityTime = 1f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Transform player) {
        playerTransform = player;
    }

    private void FixedUpdate() {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer() {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    public void TakeDamage(int damage) {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage! Health: {health}");

        if (health <= 0) {
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
            spriteRenderer.color = Color.purple;
        }
    }

    private void FlashSprite() {
        spriteRenderer.color = spriteRenderer.color == Color.white ? Color.gray : Color.white;
    }
}
