using System.Collections.Generic;
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

        foreach(DungeonSubGeneratorBase generator in subGenerators) {
            generator.SetContext(new DungeonGenerationContext {
                roomSize = config.roomSize,
                numberOfRooms = config.numberOfRooms,
            });
            generator.Run();
        }
    }

    private void ResetDungeon() {
        GameObject roomsGO = GameObject.Find("Rooms");

        if(roomsGO != null) {
            DestroyImmediate(roomsGO);
        }
    }
}
