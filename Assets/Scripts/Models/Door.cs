using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door {
    public List<Vector2Int> TilePositions { get; private set; }
    public Vector2Int Size { get; private set; }
    public Vector2Int MinBounds { get; private set; }
    public Vector2Int MaxBounds { get; private set; }

    public Door(List<Vector2Int> tilePositions) {
        if(tilePositions == null || tilePositions.Count == 0) {
            throw new System.ArgumentException("TilePositions cannot be null or empty", nameof(tilePositions));
        }

        TilePositions = tilePositions;
        CalculateBoundsAndSize();
    }

    private void CalculateBoundsAndSize() {
        int minX = TilePositions.Min(pos => pos.x);
        int maxX = TilePositions.Max(pos => pos.x);
        int minY = TilePositions.Min(pos => pos.y);
        int maxY = TilePositions.Max(pos => pos.y);

        MinBounds = new Vector2Int(minX, minY);
        MaxBounds = new Vector2Int(maxX, maxY);
        Size = new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }
}
