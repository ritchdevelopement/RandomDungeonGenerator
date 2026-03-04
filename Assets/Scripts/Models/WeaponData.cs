using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject {
    [Header("General")]
    public string weaponName;
    public int maxAmmo;
    public GameObject weaponPrefab;

    [Header("Primary \u2014 Throw")]
    public GameObject projectilePrefab;
    public float spawnOffset;
    public float primaryCooldown;

    [Header("Secondary \u2014 Melee")]
    public float meleeRange;
    public float meleeInnerRadius;
    public int meleeDamage;
    public float meleeCooldown;

    [Header("Slash Arc")]
    public Vector2 slashOriginOffset;
    public float slashArcAngle = 60f;
    public float slashAlpha = 0.6f;
    public float slashSweepDuration = 0.08f;
}
