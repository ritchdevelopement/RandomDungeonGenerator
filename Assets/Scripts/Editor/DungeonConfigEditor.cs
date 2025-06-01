using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonConfig))]
public class DungeonConfigEditor : Editor {
    public override void OnInspectorGUI() {
        DungeonConfig config = (DungeonConfig) target;

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);

        config.roomDistributionFactor = EditorGUILayout.IntSlider(
            "Room Distribution",
            config.roomDistributionFactor,
            0,
            config.numberOfRooms
        );

        EditorGUILayout.HelpBox(
            "Controls how densely rooms are allowed to cluster near the center. " +
            "Higher values result in tighter groupings, lower values encourage outward spread. " +
            "This value cannot exceed the total number of rooms.",
            MessageType.Info
        );

        if(EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(config);
        }
    }
}
