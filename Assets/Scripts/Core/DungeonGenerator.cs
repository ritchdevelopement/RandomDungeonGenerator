using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

    public DungeonConfig dungeonConfig;
    [SerializeField]
    private List<DungeonTask> dungeonTasks;

    private void Start() {
        GenerateDungeon();
    }

    public void GenerateDungeon() {
        if(dungeonConfig == null) {
            throw new MissingReferenceException($"Dungeon configuration not assigned to GameObject: {gameObject.name}");
        }

        ResetDungeon();

        DungeonGenerationContext context = new DungeonGenerationContext {
            roomSize = dungeonConfig.roomSize,
            numberOfRooms = dungeonConfig.numberOfRooms,
            roomDistributionFactor = dungeonConfig.roomDistributionFactor,
            createdRooms = new Dictionary<Vector2Int, Room>(),
            wallTile = dungeonConfig.wallTile
        };

        foreach(DungeonTask dungeonTask in dungeonTasks) {
            dungeonTask.SetContext(context);
            dungeonTask.Execute();
        }
    }

    public void SyncSubGenerators() {
        dungeonTasks = GetComponentsInChildren<DungeonTask>().ToList();
    }

    public void ResetDungeon() {
        GameObject dungeonGO = GameObject.Find("Dungeon");

        if(dungeonGO != null) {
            DestroyImmediate(dungeonGO);
        }
    }
}
