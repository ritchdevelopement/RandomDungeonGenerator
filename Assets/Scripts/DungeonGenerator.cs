using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [Header("Configuration")]
    public DungeonConfig config;

    [Header("Generators")]
    [SerializeField]
    private List<DungeonSubGeneratorBase> subGenerators;

    private void Start() {
        GenerateDungeon();
    }

    [ContextMenu("Generate Dungeon")]
    private void GenerateDungeon() {
        if(config == null) {
            throw new MissingReferenceException($"Dungeon configuration not assigned to GameObject: {gameObject.name}");
        }

        ResetDungeon();

        DungeonGenerationContext context = new DungeonGenerationContext {
            roomSize = config.roomSize,
            numberOfRooms = config.numberOfRooms,
            createdRooms = new Dictionary<Vector2Int, Room>()
        };

        foreach(DungeonSubGeneratorBase generator in subGenerators) {
            generator.SetContext(context);
            generator.Run();
        }
    }

    [ContextMenu("Sync SubGenerators")]
    private void SyncSubGenerators() {
        subGenerators = GetComponentsInChildren<DungeonSubGeneratorBase>().ToList();
    }

    private void ResetDungeon() {
        GameObject roomsGO = GameObject.Find("Rooms");

        if(roomsGO != null) {
            DestroyImmediate(roomsGO);
        }
    }
}
