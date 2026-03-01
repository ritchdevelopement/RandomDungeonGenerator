using System;
using UnityEngine.Tilemaps;

[Serializable]
public class WallTileSet {
    public TileBase cornerTopLeft;
    public TileBase cornerTopRight;
    public TileBase cornerBottomLeft;
    public TileBase cornerBottomRight;

    public TileBase wallTop;
    public TileBase wallBottom;

    public TileBase wallVerticalLeft;
    public TileBase wallVerticalRight;
}
