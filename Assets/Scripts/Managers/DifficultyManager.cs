using UnityEngine;

public class DifficultyManager : MonoBehaviour {
    public static DifficultyManager Instance { get; private set; }

    [Header("Wave")]
    [SerializeField] private int baseEnemyCount = 1;
    [SerializeField] private int additionalEnemiesPerRoom = 1;
    [SerializeField] private int additionalEnemiesPerWave = 1;
    [SerializeField] private int wavesPerRoom = 3;

    [Header("Survival")]
    [SerializeField] private float survivalDuration = 30f;
    [SerializeField] private float survivalSpawnInterval = 3f;
    [SerializeField] private int survivalInitialSpawnCount = 1;
    [SerializeField] private int survivalAdditionalEnemiesPerRoom = 1;
    [SerializeField] private int survivalSpawnCountIncrement = 1;

    public int WavesPerRoom => wavesPerRoom;
    public float SurvivalDuration => survivalDuration;
    public float SurvivalSpawnInterval => survivalSpawnInterval;

    public int GetSurvivalSpawnCount(int spawnIndex) {
        return survivalInitialSpawnCount + ClearedRoomCount * survivalAdditionalEnemiesPerRoom + spawnIndex * survivalSpawnCountIncrement;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Difficulty is driven by cleared room count and wave index — harder with each room and each wave
    private static int ClearedRoomCount => RoomManager.Instance != null
        ? RoomManager.Instance.ClearedRoomCount
        : 0;

    public int GetWaveEnemyCount(int waveIndex) {
        return baseEnemyCount + ClearedRoomCount * additionalEnemiesPerRoom + waveIndex * additionalEnemiesPerWave;
    }

    // Weighted random selection — rarity chance increases as more rooms are cleared and with each wave
    public EnemyData SelectEnemyForWave(EnemyData[] candidates, int waveIndex) {
        if (candidates == null || candidates.Length == 0) {
            return null;
        }

        float totalWeight = 0f;
        float[] weights = new float[candidates.Length];
        for (int i = 0; i < candidates.Length; i++) {
            weights[i] = candidates[i].spawnWeight * RarityMultiplier(candidates[i].rarity, waveIndex);
            totalWeight += weights[i];
        }

        float roll = Random.Range(0f, totalWeight);
        float accumulated = 0f;
        for (int i = 0; i < candidates.Length; i++) {
            accumulated += weights[i];
            if (roll <= accumulated) {
                return candidates[i];
            }
        }

        return candidates[candidates.Length - 1];
    }

    private static float RarityMultiplier(EnemyRarity rarity, int waveIndex) {
        int clearedRooms = ClearedRoomCount;
        return rarity switch {
            EnemyRarity.Normal => 1f,
            EnemyRarity.Uncommon => 0.4f + clearedRooms * 0.05f + waveIndex * 0.1f,
            EnemyRarity.Rare => 0.1f + clearedRooms * 0.03f + waveIndex * 0.05f,
            EnemyRarity.Boss => 0.02f + clearedRooms * 0.01f + waveIndex * 0.02f,
            _ => 1f
        };
    }
}
