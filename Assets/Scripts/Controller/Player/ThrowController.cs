using UnityEngine;

public class ThrowController : MonoBehaviour {
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private int maxAmmo = 3;

    public static ThrowController Instance { get; private set; }
    public static int CurrentAmmo { get; private set; }

    private int currentAmmo;

    private void Awake() {
        Instance = this;
        currentAmmo = maxAmmo;
        CurrentAmmo = currentAmmo;
    }

    private void Update() {
        if (CanThrow()) {
            ThrowProjectile();
        }
    }

    private bool CanThrow() {
        return Input.GetMouseButtonDown(0) && currentAmmo > 0;
    }

    private void ThrowProjectile() {
        Vector2 throwDirection = GetThrowDirection();
        GameObject projectile = Instantiate(throwablePrefab, transform.position, Quaternion.identity);

        if (projectile.TryGetComponent(out ThrowableProjectile throwable)) {
            throwable.Launch(throwDirection);
        }

        currentAmmo--;
        CurrentAmmo = currentAmmo;
    }

    private Vector2 GetThrowDirection() {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return ((Vector2) (mouseWorldPosition - transform.position)).normalized;
    }

    public void ReturnAmmo() {
        currentAmmo = Mathf.Min(currentAmmo + 1, maxAmmo);
        CurrentAmmo = currentAmmo;
    }
}
