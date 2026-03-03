using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ThrowableProjectile : MonoBehaviour {
    [SerializeField] private float flySpeed = 15f;

    private Rigidbody2D rigidbody2d;
    private bool isStuck = false;
    private bool collisionEnabled = false;

    private void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction) {
        transform.rotation = Quaternion.AngleAxis(CalculateRotationAngle(direction), Vector3.forward);
        rigidbody2d.linearVelocity = direction * flySpeed;
        StartCoroutine(ActivateAfterDelay());
    }

    private float CalculateRotationAngle(Vector2 direction) {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    private IEnumerator ActivateAfterDelay() {
        yield return new WaitForFixedUpdate();
        collisionEnabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!collisionEnabled) {
            return;
        }

        if (isStuck) {
            TryPickup(other);
            return;
        }

        if (IsPlayer(other)) {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(1);
        }

        Stick();
    }

    private void TryPickup(Collider2D other) {
        if (!IsPlayer(other)) {
            return;
        }

        ThrowController.Instance.ReturnAmmo();
        Destroy(gameObject);
    }

    private bool IsPlayer(Collider2D other) {
        return other.CompareTag("Player");
    }

    private void Stick() {
        isStuck = true;
        rigidbody2d.linearVelocity = Vector2.zero;
        rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
    }
}
