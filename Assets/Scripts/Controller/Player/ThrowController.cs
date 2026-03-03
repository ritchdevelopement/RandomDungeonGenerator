using UnityEngine;

public class ThrowController : MonoBehaviour {
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private int maxAmmo = 3;

    public static ThrowController Instance { get; private set; }
    public static int CurrentAmmo { get; private set; }

    private void Awake() {
        Instance = this;
        CurrentAmmo = maxAmmo;
    }

    private void Update() {
        if (CanThrow()) {
            ThrowProjectile();
        }
    }

    private bool CanThrow() {
        return Input.GetMouseButtonDown(0) && CurrentAmmo > 0;
    }

    private void ThrowProjectile() {
        Vector2 throwDirection = GetThrowDirection();
        GameObject projectile = Instantiate(throwablePrefab, transform.position, Quaternion.identity);

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
