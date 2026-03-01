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
    public TileBase outerWallTopLeftEnd;
    public TileBase outerWallTopRightEnd;
    public TileBase outerCornerTopRight;

    public TileBase outerCornerBottomLeft;
    public TileBase outerWallBottom;
    public TileBase outerWallBottomLeftEnd;
    public TileBase outerWallBottomRightEnd;
    public TileBase outerCornerBottomRight;

    public TileBase SelectWallTile(bool isTopEdge, bool isBottomEdge, bool isLeftEdge, bool isRightEdge, TileBase fallback) {
        if (isTopEdge && isLeftEdge) {
            return cornerTopLeft != null ? cornerTopLeft : fallback;
        }
        if (isTopEdge && isRightEdge) {
            return cornerTopRight != null ? cornerTopRight : fallback;
        }
        if (isBottomEdge && isLeftEdge) {
            return cornerBottomLeft != null ? cornerBottomLeft : fallback;
        }
        if (isBottomEdge && isRightEdge) {
            return cornerBottomRight != null ? cornerBottomRight : fallback;
        }
        if (isTopEdge) {
            return wallTop != null ? wallTop : fallback;
        }
        if (isBottomEdge) {
            return wallBottom != null ? wallBottom : fallback;
        }
        if (isLeftEdge) {
            return wallVerticalLeft != null ? wallVerticalLeft : fallback;
        }
        if (isRightEdge) {
            return wallVerticalRight != null ? wallVerticalRight : fallback;
        }
        return fallback;
    }

    public TileBase SelectOuterTile(bool isTopRow, bool isLeftEdge, bool isRightEdge, TileBase fallback) {
        if (isTopRow) {
            if (isLeftEdge) {
                return outerCornerTopLeft != null ? outerCornerTopLeft : fallback;
            }
            if (isRightEdge) {
                return outerCornerTopRight != null ? outerCornerTopRight : fallback;
            }
            return outerWallTop != null ? outerWallTop : fallback;
        }
        if (isLeftEdge) {
            return outerCornerBottomLeft != null ? outerCornerBottomLeft : fallback;
        }
        if (isRightEdge) {
            return outerCornerBottomRight != null ? outerCornerBottomRight : fallback;
        }
        return outerWallBottom != null ? outerWallBottom : fallback;
    }

    public TileBase SelectTransitionTile(bool isTopRow, bool isAtLeftEdge, TileBase fallback) {
        TileBase tile;
        if (isTopRow) {
            tile = isAtLeftEdge ? outerWallTopRightEnd : outerWallTopLeftEnd;
        } else {
            tile = isAtLeftEdge ? outerWallBottomRightEnd : outerWallBottomLeftEnd;
        }
        if (tile != null) {
            return tile;
        }
        return fallback;
    }
}
