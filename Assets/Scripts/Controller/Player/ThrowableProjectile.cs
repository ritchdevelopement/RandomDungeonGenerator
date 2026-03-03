using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ThrowableProjectile : MonoBehaviour {
    [SerializeField] private float flySpeed = 15f;

    private Rigidbody2D rigidbody2d;
    private bool isStuck = false;
    private bool isActive = false;

    private void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction) {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rigidbody2d.linearVelocity = direction * flySpeed;
        StartCoroutine(ActivateAfterDelay());
    }

    private IEnumerator ActivateAfterDelay() {
        yield return new WaitForFixedUpdate();
        isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!isActive) {
            return;
        }

        if (isStuck) {
            TryPickup(other);
            return;
        }

        if (other.CompareTag("Player")) {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(1);
        }

        Stick();
    }

    private void TryPickup(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        ThrowController.Instance.ReturnAmmo();
        Destroy(gameObject);
    }

    private void Stick() {
        isStuck = true;
        rigidbody2d.linearVelocity = Vector2.zero;
        rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
    }
}
