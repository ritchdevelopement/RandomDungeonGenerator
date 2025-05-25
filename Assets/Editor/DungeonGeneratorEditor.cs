using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DungeonGenerator generator = (DungeonGenerator) target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dungeon Actions", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if(GUILayout.Button("Generate Dungeon")) {
            generator.GenerateDungeon();
        }

        if(GUILayout.Button("Reset Dungeon")) {
            generator.ResetDungeon();
        }

        if(GUILayout.Button("Sync Sub Generators")) {
            generator.SyncSubGenerators();
        }
    }
}
