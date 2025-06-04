using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance { get; private set; }

    private GameObject enemyPrefab;
    private DungeonGenerationContext dungeonContext;
    private Transform playerTransform;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        if (dungeonContext.createdRooms.TryGetValue(Vector2Int.zero, out Room room)) {
            SpawnEnemy(room);
        }
    }

    public void SetEnemyPrefab(GameObject prefab) {
        enemyPrefab = prefab;
    }

    public void SetPlayer(Transform player) {
        playerTransform = player;
    }

    public void SetDungeonContext(DungeonGenerationContext context) {
        dungeonContext = context;
    }

    private void SpawnEnemy(Room room) {
        Vector3 spawnPosition = new Vector3(room.Center.x + 10, room.Center.y + 10);
        GameObject enemyGameObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, gameObject.transform);
        EnemyController enemy = enemyGameObject.GetComponent<EnemyController>();
        enemy.Initialize(playerTransform);

        Debug.Log($"Spawned enemy in room {room.Center} at {spawnPosition}");
    }
}
