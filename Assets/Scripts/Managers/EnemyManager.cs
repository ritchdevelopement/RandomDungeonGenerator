using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance { get; private set; }

    private EnemyData[] allEnemyData;
    private DungeonGenerationContext dungeonContext;
    private Transform playerTransform;

    // Tracks all living enemies per room — room is considered active until list is empty
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

    public void StartRoomEncounter(Room room, EncounterMode mode = EncounterMode.Wave) {
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

        if (mode == EncounterMode.Wave) {
            StartCoroutine(RunWaveEncounter(room));
        } else {
            StartCoroutine(RunSurvivalEncounter(room));
        }
    }

    // Spawns all enemies at once — room clears when the last one dies
    private IEnumerator RunWaveEncounter(Room room) {
        EnemyData[] factionEnemies = GetEnemiesForCurrentFaction();
        int spawnCount = DifficultyManager.Instance.GetWaveEnemyCount();
        int remainingEnemies = spawnCount;

        for (int i = 0; i < spawnCount; i++) {
            EnemyData data = DifficultyManager.Instance.SelectEnemy(factionEnemies);
            SpawnEnemy(room, data, onDeath: () => {
                remainingEnemies--;
                if (remainingEnemies <= 0) {
                    HandleEncounterCleared(room);
                }
            });
        }

        yield break;
    }

    // Spawns enemies continuously for a set duration — room clears when timer ends and all remaining enemies die
    private IEnumerator RunSurvivalEncounter(Room room) {
        EnemyData[] factionEnemies = GetEnemiesForCurrentFaction();
        float spawnInterval = DifficultyManager.Instance.GetSpawnInterval();
        float duration = DifficultyManager.Instance.GetSurvivalDuration();
        float elapsed = 0f;

        while (elapsed < duration) {
            EnemyData data = DifficultyManager.Instance.SelectEnemy(factionEnemies);
            SpawnEnemy(room, data, onDeath: null);
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        while (activeEnemies.TryGetValue(room, out List<EnemyController> enemies) && enemies.Count > 0) {
            yield return null;
        }

        HandleEncounterCleared(room);
    }

    private void SpawnEnemy(Room room, EnemyData data, System.Action onDeath) {
        if (data == null || data.prefab == null) {
            return;
        }

        List<Vector2> occupiedPositions = GetOccupiedPositions(room);
        Vector3 spawnPosition = GetRandomSpawnPosition(room, occupiedPositions);
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

    // Picks a random floor position inside the room, keeping distance from walls, center trigger, and other enemies
    private static Vector3 GetRandomSpawnPosition(Room room, List<Vector2> occupiedPositions) {
        const float wallMargin = 1.5f;
        const float centerClearance = 2f;
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
        } while (attempts < 20 && !IsValidSpawnPosition(candidate, room, occupiedPositions, centerClearance, enemySpacing));

        return new Vector3(candidate.x, candidate.y, 0f);
    }

    private static bool IsValidSpawnPosition(Vector2 candidate, Room room, List<Vector2> occupiedPositions, float centerClearance, float enemySpacing) {
        if (Vector2.Distance(candidate, room.Center) < centerClearance) {
            return false;
        }

        foreach (Vector2 occupied in occupiedPositions) {
            if (Vector2.Distance(candidate, occupied) < enemySpacing) {
                return false;
            }
        }

        return true;
    }

    private void HandleEncounterCleared(Room room) {
        activeEnemies.Remove(room);
        RoomManager.Instance.AddClearedRoom(room);
        DoorManager.Instance.OpenRoomDoors(room);
    }

    private EnemyData[] GetEnemiesForCurrentFaction() {
        if (allEnemyData == null || allEnemyData.Length == 0) {
            return new EnemyData[0];
        }

        EnemyFaction faction = WorldManager.Instance != null
            ? WorldManager.Instance.ActiveFaction
            : EnemyFaction.Undead;

        return System.Array.FindAll(allEnemyData, enemy => enemy.faction == faction);
    }
}
