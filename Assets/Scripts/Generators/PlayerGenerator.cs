using System.Linq;
using UnityEngine;

public class PlayerGenerator : DungeonTaskBase {
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private SpawnLocation spawnMode = SpawnLocation.CenterRoom;
    private GameObject spawnedPlayer;

    public override void Execute() {
        if(playerPrefab == null) {
            Debug.LogError("Player prefab not assigned to PlayerGenerator!");
            return;
        }

        GeneratePlayer();
    }

    private void GeneratePlayer() {
        Room spawnRoom = GetPlayerSpawnRoom(spawnMode);
        Vector2 spawnPosition = GetSpawnPosition(spawnRoom);

        context.playerSpawnPosition = spawnPosition;
        context.playerSpawnRoom = spawnRoom;

        CreatePlayer(spawnPosition);

        RoomManager.Instance.SetContext(context);

        Debug.Log($"Player generated at {spawnPosition} in room at {spawnRoom.Center}");
    }

    public Room GetPlayerSpawnRoom(SpawnLocation spawnLocation) {
        var rooms = context.createdRooms.Values.ToArray();
        if(rooms.Length == 0) {
            Debug.LogWarning("No rooms available for player spawn selection!");
            return null;
        }

        return spawnLocation switch {
            SpawnLocation.CenterRoom => GetCenterRoom(rooms),
            SpawnLocation.RandomRoom => GetRandomRoom(rooms),
            SpawnLocation.EdgeRoom => GetEdgeRoom(rooms),
            _ => throw new System.ArgumentOutOfRangeException(
                nameof(spawnLocation),
                $"Unsupported spawn location: {spawnLocation}"
            )
        };
    }

    public Room GetCenterRoom(Room[] rooms) {
        return rooms
            .OrderBy(room => Vector2Int.Distance(room.Center, Vector2Int.zero))
            .First();
    }

    public Room GetRandomRoom(Room[] rooms) {
        return rooms[Random.Range(0, rooms.Length)];
    }

    public Room GetEdgeRoom(Room[] rooms) {
        return rooms
            .OrderByDescending(room => Vector2Int.Distance(room.Center, Vector2Int.zero))
            .First();
    }

    private Vector2 GetSpawnPosition(Room room) {
        return new Vector2(room.Center.x, room.Center.y);
    }

    private void CreatePlayer(Vector2 spawnPosition) {
        Vector3 spawnPos3D = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        spawnedPlayer = Instantiate(playerPrefab, spawnPos3D, Quaternion.identity);
        spawnedPlayer.name = "Player";
        spawnedPlayer.transform.SetParent(context.dungeonGameObject.transform);

        FindFirstObjectByType<CameraController>().SetPlayer(spawnedPlayer.transform);

        EnemyManager.Instance.SetPlayer(spawnedPlayer.transform);
        RoomManager.Instance.SetPlayer(spawnedPlayer.transform);
    }
}
