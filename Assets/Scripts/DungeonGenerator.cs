using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public Vector2Int roomSize;
    public int numberOfRooms;
    public GameObject wallTile;

    [SerializeField]
    private RoomGenerator roomGenerator;

    private void Start() {
        GenerateDungeon();
    }

    [ContextMenu("Generate Dungeon")]
    private void GenerateDungeon() {
        ResetDungeon();

        RoomGenerator roomGenerator = new RoomGenerator(wallTile, numberOfRooms, roomSize);
        roomGenerator.Run();
    }

    private void ResetDungeon() {
        DestroyImmediate(GameObject.Find("Rooms"));
    }
}
