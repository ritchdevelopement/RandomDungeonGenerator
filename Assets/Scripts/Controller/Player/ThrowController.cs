using UnityEngine;

public class ThrowController : MonoBehaviour {
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private float projectileSpawnOffset = 0.4f;

    public static ThrowController Instance { get; private set; }
    public static int CurrentAmmo { get; private set; }
    public static int MaxAmmo { get; private set; }
    public static Sprite WeaponSprite { get; private set; }

    private void Awake() {
        Instance = this;
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
        if (throwablePrefab != null) {
            SpriteRenderer throwableSpriteRenderer = throwablePrefab.GetComponent<SpriteRenderer>();
            WeaponSprite = throwableSpriteRenderer != null ? throwableSpriteRenderer.sprite : null;
        }
    }

    private void Update() {
        if (CanThrow()) {
            ThrowProjectile();
        }
    }

    private bool CanThrow() {
        return Input.GetMouseButtonDown(0) && CurrentAmmo > 0 && Time.timeScale > 0f;
    }

    private void ThrowProjectile() {
        Vector2 throwDirection = GetThrowDirection();
        Vector3 spawnPosition = transform.position + (Vector3) (throwDirection * projectileSpawnOffset);
        GameObject projectile = Instantiate(throwablePrefab, spawnPosition, Quaternion.identity);

        if (projectile.TryGetComponent(out ThrowableProjectile throwable)) {
            throwable.Launch(throwDirection);
        }

        ConsumeAmmo();
    }

    private Vector2 GetThrowDirection() {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (Vector2) (mouseWorldPosition - transform.position);
        return directionToMouse.normalized;
    }

    private void ConsumeAmmo() {
        CurrentAmmo--;
    }

    public void ReturnAmmo() {
        CurrentAmmo = Mathf.Min(CurrentAmmo + 1, maxAmmo);
    }
}
