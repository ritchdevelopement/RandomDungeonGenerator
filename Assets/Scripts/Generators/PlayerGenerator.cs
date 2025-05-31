using System.Linq;
using UnityEngine;

public class PlayerGenerator : DungeonTaskBase {
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool spawnInCenterRoom = true;
    private GameObject spawnedPlayer;

    public override void Execute() {
        if(playerPrefab == null) {
            Debug.LogError("Player prefab not assigned to PlayerGenerator!");
            return;
        }

        if(context.createdRooms == null || context.createdRooms.Count == 0) {
            Debug.LogError("No rooms available for player generation!");
            return;
        }

        GeneratePlayer();
    }

    private void GeneratePlayer() {
        DestroyExistingPlayer();

        Room spawnRoom = GetSpawnRoom();
        Vector2 spawnPosition = GetSpawnPosition(spawnRoom);

        context.playerSpawnPosition = spawnPosition;
        context.playerSpawnRoom = spawnRoom;

        CreatePlayer(spawnPosition);

        Debug.Log($"Player generated at {spawnPosition} in room at {spawnRoom.Center}");
    }

    private Room GetSpawnRoom() {
        if(spawnInCenterRoom) {
            return context.createdRooms.Values
                .OrderBy(room => Vector2Int.Distance(room.Center, Vector2Int.zero))
                .First();
        }

        return context.createdRooms.Values.First();
    }

    private Vector2 GetSpawnPosition(Room room) {
        Vector2 roomCenter = new Vector2(room.Center.x, room.Center.y);

        return roomCenter;
    }

    private void CreatePlayer(Vector2 spawnPosition) {
        Vector3 spawnPos3D = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        spawnedPlayer = Instantiate(playerPrefab, spawnPos3D, Quaternion.identity);
        spawnedPlayer.name = "Player";
        spawnedPlayer.transform.SetParent(context.dungeonGameObject.transform);
    }

    private void DestroyExistingPlayer() {
        if(spawnedPlayer != null) {
            DestroyImmediate(spawnedPlayer);
        }

        PlayerController existingPlayer = FindFirstObjectByType<PlayerController>();
        if(existingPlayer != null) {
            DestroyImmediate(existingPlayer.gameObject);
        }
    }

    public GameObject GetSpawnedPlayer() {
        return spawnedPlayer;
    }

    public void RespawnPlayer() {
        if(context != null && context.createdRooms.Count > 0) {
            GeneratePlayer();
        }
    }
}
