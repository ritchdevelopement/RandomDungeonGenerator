using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    [Header("Weapon")]
    [SerializeField] private WeaponData startingWeapon;

    [Header("Slash Arc")]
    [SerializeField] private Material slashMaterial;
    [SerializeField] private int slashSortingOrder = 100;

    private const int SlashSegments = 12;
    private const float SlashFadeDuration = 0.1f;

    private Camera mainCamera;
    private float primaryCooldownRemaining;
    private float secondaryCooldownRemaining;

    public static WeaponController Instance { get; private set; }
    public static WeaponData CurrentWeapon { get; private set; }
    public static int CurrentAmmo { get; private set; }
    public static int MaxAmmo { get; private set; }
    public static Sprite WeaponSprite {
        get {
            if (CurrentWeapon == null || CurrentWeapon.weaponPrefab == null) {
                return null;
            }
            SpriteRenderer spriteRenderer = CurrentWeapon.weaponPrefab.GetComponent<SpriteRenderer>();
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }
    }
    public static event System.Action OnWeaponChanged;

    private void Awake() {
        Instance = this;
        mainCamera = Camera.main;
        Equip(startingWeapon);
    }

    private void Update() {
        TickCooldowns();

        if (CanUsePrimary()) {
            ExecutePrimaryAttack();
        }

        if (CanUseSecondary()) {
            ExecuteSecondaryAttack();
        }
    }

    public void Equip(WeaponData weapon) {
        CurrentWeapon = weapon;
        MaxAmmo = weapon.maxAmmo;
        CurrentAmmo = weapon.maxAmmo;
        primaryCooldownRemaining = 0f;
        secondaryCooldownRemaining = 0f;
        OnWeaponChanged?.Invoke();
    }

    public void ReturnAmmo() {
        CurrentAmmo = Mathf.Min(CurrentAmmo + 1, MaxAmmo);
    }

    public void AddMaxAmmo(int amount) {
        MaxAmmo += amount;
        CurrentAmmo = Mathf.Min(CurrentAmmo + amount, MaxAmmo);
        OnWeaponChanged?.Invoke();
    }

    private bool CanUsePrimary() {
        return Input.GetMouseButtonDown(0) && IsAttackReady(primaryCooldownRemaining);
    }

    private bool CanUseSecondary() {
        return Input.GetMouseButtonDown(1) && IsAttackReady(secondaryCooldownRemaining);
    }

    private bool IsAttackReady(float cooldownRemaining) {
        return CurrentAmmo > 0
            && cooldownRemaining <= 0f
            && Time.timeScale > 0f;
    }

    private void ExecutePrimaryAttack() {
        primaryCooldownRemaining = CurrentWeapon.primaryCooldown;

        Vector2 throwDirection = GetDirectionToMouse();
        Vector3 spawnPosition = transform.position + (Vector3) (throwDirection * CurrentWeapon.spawnOffset);
        GameObject projectile = Instantiate(CurrentWeapon.projectilePrefab, spawnPosition, Quaternion.identity);

        if (projectile.TryGetComponent(out ThrowableProjectile throwable)) {
            throwable.Launch(throwDirection);
        }

        CurrentAmmo--;
    }

    private void ExecuteSecondaryAttack() {
        secondaryCooldownRemaining = CurrentWeapon.meleeCooldown;

        Vector2 attackDirection = GetDirectionToMouse();
        Vector2 meleeCenter = GetMeleeCenter();
        float attackAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleeCenter, CurrentWeapon.meleeRange);
        foreach (Collider2D hit in hits) {
            if (hit.gameObject == gameObject) {
                continue;
            }

            if (!IsWithinMeleeArc(hit, meleeCenter, attackAngle)) {
                continue;
            }

            hit.GetComponentInParent<IDamageable>()?.TakeDamage(CurrentWeapon.meleeDamage);
        }

        StartCoroutine(DrawSlashArc(attackDirection, meleeCenter));
    }

    private bool IsWithinMeleeArc(Collider2D hit, Vector2 meleeCenter, float attackAngle) {
        Vector2 directionToHit = (Vector2) hit.bounds.center - meleeCenter;
        float angleToHit = Mathf.Atan2(directionToHit.y, directionToHit.x) * Mathf.Rad2Deg;
        return Mathf.Abs(Mathf.DeltaAngle(attackAngle, angleToHit)) <= CurrentWeapon.slashArcAngle * 0.5f;
    }

    private Vector2 GetMeleeCenter() {
        return (Vector2) transform.position + CurrentWeapon.slashOriginOffset;
    }

    private Vector2 GetDirectionToMouse() {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (Vector2) (mouseWorldPosition - transform.position);
        return directionToMouse.normalized;
    }

    private void TickCooldowns() {
        if (primaryCooldownRemaining > 0f) {
            primaryCooldownRemaining -= Time.deltaTime;
        }

        if (secondaryCooldownRemaining > 0f) {
            secondaryCooldownRemaining -= Time.deltaTime;
        }
    }

    private IEnumerator DrawSlashArc(Vector2 direction, Vector2 center) {
        GameObject slashObject = new("SlashEffect");
        MeshFilter meshFilter = slashObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = slashObject.AddComponent<MeshRenderer>();

        Material instanceMaterial = new(slashMaterial);
        meshRenderer.material = instanceMaterial;
        meshRenderer.sortingOrder = slashSortingOrder;
        Color baseColor = CurrentWeapon.slashColor;
        instanceMaterial.color = new Color(baseColor.r, baseColor.g, baseColor.b, CurrentWeapon.slashAlpha);

        Mesh mesh = new();
        meshFilter.mesh = mesh;

        Vector3[] allVertices = BuildSlashVertices(direction, center);
        yield return AnimateSweep(mesh, allVertices);
        yield return AnimateFade(instanceMaterial);

        Destroy(slashObject);
    }

    private Vector3[] BuildSlashVertices(Vector2 direction, Vector3 center) {
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - CurrentWeapon.slashArcAngle * 0.5f;

        // sweep runs right to left: angle decreases with index
        // inner radius tapers from meleeRange (tip) to meleeInnerRadius (full width at tail)
        Vector3[] allVertices = new Vector3[(SlashSegments + 1) * 2];
        for (int i = 0; i <= SlashSegments; i++) {
            float angle = (baseAngle + CurrentWeapon.slashArcAngle * (1f - (float) i / SlashSegments)) * Mathf.Deg2Rad;
            Vector3 arcDirection = new(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            float t = (float) i / SlashSegments;
            float innerRadius = Mathf.Lerp(CurrentWeapon.meleeRange, CurrentWeapon.meleeInnerRadius, t);
            allVertices[i] = center + arcDirection * innerRadius;
            allVertices[SlashSegments + 1 + i] = center + arcDirection * CurrentWeapon.meleeRange;
        }

        return allVertices;
    }

    private IEnumerator AnimateSweep(Mesh mesh, Vector3[] allVertices) {
        float elapsed = 0f;

        while (elapsed < CurrentWeapon.slashSweepDuration) {
            int visibleSegments = Mathf.Max(1, Mathf.CeilToInt(elapsed / CurrentWeapon.slashSweepDuration * SlashSegments));
            UpdateMeshSegments(mesh, allVertices, visibleSegments);
            elapsed += Time.deltaTime;
            yield return null;
        }

        UpdateMeshSegments(mesh, allVertices, SlashSegments);
    }

    private IEnumerator AnimateFade(Material instanceMaterial) {
        Color baseColor = CurrentWeapon.slashColor;
        float elapsed = 0f;

        while (elapsed < SlashFadeDuration) {
            float alpha = (1f - elapsed / SlashFadeDuration) * CurrentWeapon.slashAlpha;
            instanceMaterial.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void UpdateMeshSegments(Mesh mesh, Vector3[] allVertices, int segmentCount) {
        // allVertices layout: inner[0..SlashSegments], outer[SlashSegments+1..2*SlashSegments+1]
        Vector3[] vertices = new Vector3[(segmentCount + 1) * 2];
        System.Array.Copy(allVertices, 0, vertices, 0, segmentCount + 1);
        System.Array.Copy(allVertices, SlashSegments + 1, vertices, segmentCount + 1, segmentCount + 1);

        int[] triangles = new int[segmentCount * 6];
        for (int i = 0; i < segmentCount; i++) {
            int innerLeft = i;
            int innerRight = i + 1;
            int outerLeft = segmentCount + 1 + i;
            int outerRight = segmentCount + 1 + i + 1;

            triangles[i * 6] = innerLeft;
            triangles[i * 6 + 1] = outerLeft;
            triangles[i * 6 + 2] = innerRight;
            triangles[i * 6 + 3] = innerRight;
            triangles[i * 6 + 4] = outerLeft;
            triangles[i * 6 + 5] = outerRight;
        }

        // Vertex colors: inner edge fully transparent, outer fades from tip (alpha 0) to tail (alpha 1)
        Color[] colors = new Color[(segmentCount + 1) * 2];
        for (int i = 0; i <= segmentCount; i++) {
            float t = (float) i / segmentCount;
            colors[i] = Color.clear;
            colors[segmentCount + 1 + i] = new Color(1f, 1f, 1f, t);
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
    }
}
