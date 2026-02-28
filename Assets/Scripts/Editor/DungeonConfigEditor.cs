using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonConfig))]
public class DungeonConfigEditor : Editor {
    public override void OnInspectorGUI() {
        DungeonConfig config = (DungeonConfig) target;

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(config);
        }
    }
}
