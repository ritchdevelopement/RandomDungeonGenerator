using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ThrowableProjectile : MonoBehaviour {
    [SerializeField] private float flySpeed = 15f;
    [SerializeField] private float stuckSpreadRadius = 0.15f;

    private Rigidbody2D rigidbody2d;
    private bool isStuck;
    private bool isPiercing;
    private EnemyController stuckEnemy;

    private void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, bool piercing = false) {
        isPiercing = piercing;
        transform.rotation = Quaternion.AngleAxis(CalculateRotationAngle(direction), Vector3.forward);
        rigidbody2d.linearVelocity = direction * flySpeed;
    }

    private float CalculateRotationAngle(Vector2 direction) {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (isStuck) {
            TryPickup(other);
            return;
        }

        if (IsPlayer(other)) {
            return;
        }

        // Must stick before dealing damage so DetachFromEnemy runs before the enemy is destroyed
        if (other.TryGetComponent(out EnemyController enemy)) {
            if (!isPiercing) {
                StickToEnemy(enemy);
            }
        } else {
            Stick();
        }

        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(1);
        }
    }

    private void TryPickup(Collider2D other) {
        if (!IsPlayer(other)) {
            return;
        }

        if (stuckEnemy != null) {
            stuckEnemy.OnDeath -= DetachFromEnemy;
        }
        WeaponController.Instance.ReturnAmmo();
        Destroy(gameObject);
    }

    private bool IsPlayer(Collider2D other) {
        return other.CompareTag("Player");
    }

    private void StickToEnemy(EnemyController enemy) {
        stuckEnemy = enemy;
        Stick();
        transform.SetParent(enemy.transform);
        transform.localPosition += (Vector3) (Random.insideUnitCircle * stuckSpreadRadius);
        enemy.OnDeath += DetachFromEnemy;
    }

    private void DetachFromEnemy() {
        transform.SetParent(null);
    }

    private void Stick() {
        isStuck = true;
        rigidbody2d.linearVelocity = Vector2.zero;
        rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
    }
}
