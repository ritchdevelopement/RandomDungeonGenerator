using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(DungeonConfig))]
public class DungeonConfigEditor : Editor {
    public override void OnInspectorGUI() {
        DungeonConfig config = (DungeonConfig) target;

        EditorGUI.BeginChangeCheck();

        config.roomSize = EditorGUILayout.Vector2IntField("Room Size", config.roomSize);
        config.numberOfRooms = EditorGUILayout.IntField("Number of Rooms", config.numberOfRooms);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        config.roomDistributionFactor = EditorGUILayout.IntSlider(
            "Room Distribution",
            config.roomDistributionFactor,
            0,
            config.numberOfRooms
        );

        if(EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.HelpBox(
            "Controls how densely rooms are allowed to cluster near the center. " +
            "Higher values result in tighter groupings, lower values encourage outward spread. " +
            "This value cannot exceed the total number of rooms.",
            MessageType.Info
        );
    }
}
