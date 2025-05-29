using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonComposer : MonoBehaviour {

    [SerializeField]
    private DungeonConfig dungeonConfig;

    [SerializeField]
    private List<DungeonTaskBase> dungeonTasks;

    private void Start() {
        ComposeDungeon();
    }

    public void ComposeDungeon() {
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

        foreach(DungeonTaskBase dungeonTask in dungeonTasks) {
            dungeonTask.SetContext(context);
            dungeonTask.Execute();
        }
    }

    public void SyncDungeonTasks() {
        dungeonTasks = GetComponentsInChildren<DungeonTaskBase>().ToList();
    }

    public void ResetDungeon() {
        GameObject dungeonGameObject = GameObject.Find("Dungeon");

        if(dungeonGameObject != null) {
            DestroyImmediate(dungeonGameObject);
        }
    }
}
