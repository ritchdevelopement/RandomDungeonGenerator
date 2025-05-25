using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

    public DungeonConfig dungeonConfig;
    [SerializeField]
    private List<DungeonSubGeneratorBase> subGenerators;

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
            createdRooms = new Dictionary<Vector2Int, Room>()
        };

        foreach(DungeonSubGeneratorBase generator in subGenerators) {
            generator.SetContext(context);
            generator.Run();
        }
    }

    public void SyncSubGenerators() {
        subGenerators = GetComponentsInChildren<DungeonSubGeneratorBase>().ToList();
    }

    public void ResetDungeon() {
        GameObject roomsGO = GameObject.Find("Rooms");

        if(roomsGO != null) {
            DestroyImmediate(roomsGO);
        }
    }
}
