using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonConfig))]
public class DungeonConfigEditor : Editor {
    public override void OnInspectorGUI() {
        DungeonConfig config = (DungeonConfig) target;

        EditorGUI.BeginChangeCheck();

        config.roomSize = EditorGUILayout.Vector2IntField("Room Size", config.roomSize);
        config.numberOfRooms = EditorGUILayout.IntField("Number of Rooms", config.numberOfRooms);

        config.roomDistributionFactor = EditorGUILayout.IntSlider(
            "Room Distribution",
            config.roomDistributionFactor,
            1,
            Mathf.Max(1, config.numberOfRooms)
        );

        if(EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(config);
        }
    }
}
