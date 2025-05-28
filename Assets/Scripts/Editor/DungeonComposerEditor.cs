using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonComposer))]
public class DungeonComposerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DungeonComposer composer = (DungeonComposer) target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dungeon Actions", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if(GUILayout.Button("Compose Dungeon")) {
            composer.ComposeDungeon();
        }

        if(GUILayout.Button("Reset Dungeon")) {
            composer.ResetDungeon();
        }

        if(GUILayout.Button("Sync Dungeon Tasks")) {
            composer.SyncDungeonTasks();
        }
    }
}
