using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Config")]
public class DungeonConfig : ScriptableObject {
    [Header("Dungeon Data")]
    public List<Vector2Int> roomSizes = new() { new Vector2Int(33, 19) };
    public int numberOfRooms = 25;

    [Range(0f, 1f)]
    public float distributionBias = 0.25f;

    [Header("Materials")]
    public PhysicsMaterial2D frictionlessMaterial;
}
