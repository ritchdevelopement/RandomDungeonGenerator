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

    public TileBase outerCornerTopLeft;
    public TileBase outerWallTop;
    public TileBase outerCornerTopRight;

    public TileBase outerCornerBottomLeft;
    public TileBase outerWallBottom;
    public TileBase outerCornerBottomRight;
}
