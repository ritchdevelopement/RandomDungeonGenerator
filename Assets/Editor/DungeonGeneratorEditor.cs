using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DungeonGenerator generator = (DungeonGenerator) target;

        GUILayout.Space(10);

        if(GUILayout.Button("Generate Dungeon")) {
            generator.GenerateDungeon();
        }

        if(GUILayout.Button("Sync Sub Generators")) {
            generator.SyncSubGenerators();
        }
    }
}
