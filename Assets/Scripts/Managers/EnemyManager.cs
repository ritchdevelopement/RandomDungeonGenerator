using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance { get; private set; }

    private GameObject enemyPrefab;
    private DungeonGenerationContext dungeonContext;
    private Transform playerTransform;
    private Dictionary<Room, EnemyController> activeEnemies = new();

    private void Awake() {
        if(Instance == null) {
            Instance = this;
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

    public void OnPlayerEnterRoom(Room room) {
        if(room == null || activeEnemies.ContainsKey(room)) return;

        if(room == dungeonContext.playerSpawnRoom) {
            DoorManager.Instance.OpenRoomDoors(room);
            return;
        }

        SpawnEnemy(room);
        DoorManager.Instance.CloseRoomDoors(room);
    }

    private void SpawnEnemy(Room room) {
        Vector3 spawnPosition = new Vector3(room.Center.x + 5, room.Center.y + 5);
        GameObject enemyGameObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, gameObject.transform);
        EnemyController enemy = enemyGameObject.GetComponent<EnemyController>();
        enemy.Initialize(playerTransform);
        enemy.OnDeath += () => HandleEnemyDeath(room);
        activeEnemies[room] = enemy;

        Debug.Log($"Spawned enemy in room {room.Center} at {spawnPosition}");
    }

    private void HandleEnemyDeath(Room room) {
        RoomManager.Instance.AddClearedRoom(room);
        activeEnemies.Remove(room);
        DoorManager.Instance.OpenRoomDoors(room);
    }
}
