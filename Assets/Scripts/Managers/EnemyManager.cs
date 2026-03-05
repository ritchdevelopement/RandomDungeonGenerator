using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance { get; private set; }

    public static event System.Action<string> OnEncounterInfoChanged;

    private EnemyData[] allEnemyData;
    private DungeonGenerationContext dungeonContext;
    private Transform playerTransform;

    private Dictionary<Room, List<EnemyController>> activeEnemies = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void SetEnemyData(EnemyData[] data) {
        allEnemyData = data;
    }

    public void SetPlayer(Transform player) {
        playerTransform = player;
    }

    public void SetDungeonContext(DungeonGenerationContext context) {
        dungeonContext = context;
    }

    public void StartRoomEncounter(Room room, RoomEventType eventType) {
        if (!Application.isPlaying) {
            return;
        }

        if (room == null || dungeonContext == null || activeEnemies.ContainsKey(room)) {
            return;
        }

        if (room == dungeonContext.playerSpawnRoom) {
            DoorManager.Instance.OpenRoomDoors(room);
            return;
        }

        activeEnemies[room] = new List<EnemyController>();
        DoorManager.Instance.CloseRoomDoors(room);

        if (eventType == RoomEventType.Survival) {
            StartCoroutine(RunSurvivalEncounter(room));
        } else {
            StartCoroutine(RunMultiWaveEncounter(room));
        }
    }

    private IEnumerator RunMultiWaveEncounter(Room room) {
        EnemyData[] factionEnemies = GetEnemiesForCurrentFaction();
        int totalWaves = DifficultyManager.Instance.WavesPerRoom;

        for (int waveIndex = 0; waveIndex < totalWaves; waveIndex++) {
            OnEncounterInfoChanged?.Invoke($"Wave {waveIndex + 1} / {totalWaves}");

            int spawnCount = DifficultyManager.Instance.GetWaveEnemyCount(waveIndex);
            int remaining = spawnCount;
            bool waveCleared = false;

            for (int i = 0; i < spawnCount; i++) {
                EnemyData data = DifficultyManager.Instance.SelectEnemyForWave(factionEnemies, waveIndex);
                SpawnEnemy(room, data, onDeath: () => {
                    remaining--;
                    if (remaining <= 0) {
                        waveCleared = true;
                    }
                });
            }

            yield return new WaitUntil(() => waveCleared);
        }

        TransitionToPerkSelection(room);
    }

    private IEnumerator RunSurvivalEncounter(Room room) {
        EnemyData[] factionEnemies = GetEnemiesForCurrentFaction();
        float duration = DifficultyManager.Instance.SurvivalDuration;
        float spawnInterval = DifficultyManager.Instance.SurvivalSpawnInterval;
        float elapsed = 0f;
        float timeSinceLastSpawn = spawnInterval; // pre-filled so the first wave spawns immediately
        int spawnIndex = 0;

        while (elapsed < duration) {
            float remaining = duration - elapsed;
            int minutes = (int) (remaining / 60f);
            int seconds = (int) (remaining % 60f);
            OnEncounterInfoChanged?.Invoke($"Survive: {minutes}:{seconds:00}");

            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= spawnInterval) {
                timeSinceLastSpawn = 0f;
                int spawnCount = DifficultyManager.Instance.GetSurvivalSpawnCount(spawnIndex);
                for (int i = 0; i < spawnCount; i++) {
                    EnemyData data = DifficultyManager.Instance.SelectEnemyForWave(factionEnemies, spawnIndex);
                    SpawnEnemy(room, data, onDeath: () => { });
                }
                spawnIndex++;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        TransitionToPerkSelection(room);
    }

    private void TransitionToPerkSelection(Room room) {
        OnEncounterInfoChanged?.Invoke(null);
        PerkManager.Instance.ShowPerkSelectionUI(room);
    }

    public void FinalizeEncounter(Room room) {
        activeEnemies.Remove(room);
        RoomManager.Instance.AddClearedRoom(room);
        DoorManager.Instance.OpenRoomDoors(room);
    }

    private void SpawnEnemy(Room room, EnemyData data, System.Action onDeath) {
        if (data == null || data.prefab == null) {
            onDeath?.Invoke();
            return;
        }

        List<Vector2> occupiedPositions = GetOccupiedPositions(room);
        Vector2 playerPosition = playerTransform != null ? (Vector2) playerTransform.position : (Vector2) room.Center;
        Vector3 spawnPosition = GetRandomSpawnPosition(room, occupiedPositions, playerPosition);
        GameObject enemyObject = Instantiate(data.prefab, spawnPosition, Quaternion.identity, transform);
        EnemyController enemy = enemyObject.GetComponent<EnemyController>();
        enemy.Initialize(playerTransform, data);
        enemy.OnDeath += () => {
            if (activeEnemies.TryGetValue(room, out List<EnemyController> enemies)) {
                enemies.Remove(enemy);
            }
            onDeath?.Invoke();
        };

        activeEnemies[room].Add(enemy);
    }

    private List<Vector2> GetOccupiedPositions(Room room) {
        List<Vector2> positions = new List<Vector2>();
        if (activeEnemies.TryGetValue(room, out List<EnemyController> enemies)) {
            foreach (EnemyController enemy in enemies) {
                if (enemy != null) {
                    positions.Add(enemy.transform.position);
                }
            }
        }
        return positions;
    }

    private static Vector3 GetRandomSpawnPosition(Room room, List<Vector2> occupiedPositions, Vector2 playerPosition) {
        const float wallMargin = 1.5f;
        const float playerClearance = 3f;
        const float enemySpacing = 1.5f;

        float xMin = room.Bounds.xMin + wallMargin;
        float xMax = room.Bounds.xMax - wallMargin;
        float yMin = room.Bounds.yMin + wallMargin;
        float yMax = room.Bounds.yMax - wallMargin;

        Vector2 candidate;
        int attempts = 0;
        do {
            float x = Random.Range(xMin, xMax);
            float y = Random.Range(yMin, yMax);
            candidate = new Vector2(x, y);
            attempts++;
        } while (attempts < 20 && !IsValidSpawnPosition(candidate, occupiedPositions, playerPosition, playerClearance, enemySpacing));

        return new Vector3(candidate.x, candidate.y, 0f);
    }

    private static bool IsValidSpawnPosition(Vector2 candidate, List<Vector2> occupiedPositions, Vector2 playerPosition, float playerClearance, float enemySpacing) {
        if (Vector2.Distance(candidate, playerPosition) < playerClearance) {
            return false;
        }

        foreach (Vector2 occupied in occupiedPositions) {
            if (Vector2.Distance(candidate, occupied) < enemySpacing) {
                return false;
            }
        }

        return true;
    }

    private EnemyData[] GetEnemiesForCurrentFaction() {
        if (allEnemyData == null || allEnemyData.Length == 0) {
            return System.Array.Empty<EnemyData>();
        }

        EnemyFaction faction = WorldManager.Instance != null
            ? WorldManager.Instance.ActiveFaction
            : EnemyFaction.Undead;

        return System.Array.FindAll(allEnemyData, enemy => enemy.faction == faction);
    }
}
