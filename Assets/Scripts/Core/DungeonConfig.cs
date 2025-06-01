using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Config")]
public class DungeonConfig : ScriptableObject {
    [Header("Dungeon Data")]
    public Vector2Int roomSize = new(32, 18);
    public int numberOfRooms = 25;

    [HideInInspector]
    public int roomDistributionFactor = 0;

    [Header("Materials")]
    public PhysicsMaterial2D frictionlessMaterial;
}
