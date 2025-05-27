using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomRendererTilemap : MonoBehaviour {
    public TileBase wallTile;
    public Material tilemapMaterial;
    private string tilemapName = "WallsTilemap";

    [ContextMenu("Initialize Tilemap")]
    void Start() {
        GameObject gridGO = new GameObject("Grid");
        Grid grid = gridGO.AddComponent<Grid>();
        grid.cellSize = new Vector3Int(1, 1, 0);

        GameObject tilemapGO = new GameObject(tilemapName);
        tilemapGO.transform.parent = gridGO.transform;

        Tilemap tilemap = tilemapGO.AddComponent<Tilemap>();
        TilemapRenderer renderer = tilemapGO.AddComponent<TilemapRenderer>();
        renderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;

        tilemap.SetTile(new Vector3Int(0, 0, 0), wallTile);
    }
}
