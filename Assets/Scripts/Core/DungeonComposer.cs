using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonComposer : MonoBehaviour {

    [SerializeField]
    private DungeonConfig dungeonConfig;

    [SerializeField]
    private List<DungeonTaskBase> dungeonTasks;

    private GameObject dungeonGameObject;

    private void Start() {
        ComposeDungeon();
    }

    public void ComposeDungeon() {
        if(dungeonConfig == null) {
            throw new MissingReferenceException($"Dungeon configuration not assigned to GameObject: {gameObject.name}");
        }

        ResetDungeon();
        CreateDungeonBase();

        DungeonGenerationContext context = new DungeonGenerationContext {
            roomSize = dungeonConfig.roomSize,
            numberOfRooms = dungeonConfig.numberOfRooms,
            roomDistributionFactor = dungeonConfig.roomDistributionFactor,
            createdRooms = new Dictionary<Vector2Int, Room>(),
            dungeonGameObject = dungeonGameObject,
            frictionlessMaterial = dungeonConfig.frictionlessMaterial
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
        if(dungeonGameObject == null) {
            return;
        }

        DestroyImmediate(dungeonGameObject);
    }

    public void CreateDungeonBase() {
        dungeonGameObject = new GameObject("Dungeon");
        Grid dungeonGrid = dungeonGameObject.AddComponent<Grid>();
        dungeonGrid.cellSize = new Vector3Int(1, 1, 0);
    }
}
