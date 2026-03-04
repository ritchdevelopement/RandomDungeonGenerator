using UnityEngine;

public class DifficultyManager : MonoBehaviour {
    public static DifficultyManager Instance { get; private set; }

    [Header("Wave")]
    [SerializeField] private int baseEnemyCount = 1;
    [SerializeField] private int additionalEnemiesPerRoom = 1;

    [Header("Survival")]
    [SerializeField] private float baseSurvivalDuration = 20f;
    [SerializeField] private float additionalDurationPerRoom = 2f;
    [SerializeField] private float baseSpawnInterval = 3f;
    [SerializeField] private float spawnIntervalReductionPerRoom = 0.1f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;

    public const int PerkChoiceCount = 3;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Difficulty is driven by cleared room count — harder with each room, perks help the player keep up
    private static int ClearedRoomCount => RoomManager.Instance != null
        ? RoomManager.Instance.ClearedRoomCount
        : 0;

    public int GetWaveEnemyCount() {
        return baseEnemyCount + ClearedRoomCount * additionalEnemiesPerRoom;
    }

    public float GetSurvivalDuration() {
        return baseSurvivalDuration + ClearedRoomCount * additionalDurationPerRoom;
    }

    public float GetSpawnInterval() {
        return Mathf.Max(minimumSpawnInterval, baseSpawnInterval - ClearedRoomCount * spawnIntervalReductionPerRoom);
    }

    // Weighted random selection — rarity chance increases as more rooms are cleared
    public EnemyData SelectEnemy(EnemyData[] candidates) {
        if (candidates == null || candidates.Length == 0)
            return null;

        float totalWeight = 0f;
        float[] weights = new float[candidates.Length];
        for (int i = 0; i < candidates.Length; i++) {
            weights[i] = candidates[i].spawnWeight * RarityMultiplier(candidates[i].rarity);
            totalWeight += weights[i];
        }

        float roll = Random.Range(0f, totalWeight);
        float accumulated = 0f;
        for (int i = 0; i < candidates.Length; i++) {
            accumulated += weights[i];
            if (roll <= accumulated)
                return candidates[i];
        }

        return candidates[candidates.Length - 1];
    }

    private static float RarityMultiplier(EnemyRarity rarity) {
        int clearedRooms = ClearedRoomCount;
        return rarity switch {
            EnemyRarity.Normal => 1f,
            EnemyRarity.Uncommon => 0.4f + clearedRooms * 0.05f,
            EnemyRarity.Rare => 0.1f + clearedRooms * 0.03f,
            EnemyRarity.Boss => 0.02f + clearedRooms * 0.01f,
            _ => 1f
        };
    }
}
