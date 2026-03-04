using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject {
    [Header("Identity")]
    public string enemyName;
    public EnemyFaction faction;
    public EnemyRarity rarity;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Stats")]
    public int health = 5;
    public float moveSpeed = 2f;
    public int contactDamage = 1;

    [Header("Spawn Weight")]
    [Tooltip("Relative probability of this enemy being selected. Higher = more common.")]
    [Range(0f, 1f)]
    public float spawnWeight = 1f;
}
