using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Config")]
public class DungeonConfig : ScriptableObject {
    public Vector2Int roomSize = new(32, 18);
    public int numberOfRooms = 25;
    public GameObject wallTile;
}
