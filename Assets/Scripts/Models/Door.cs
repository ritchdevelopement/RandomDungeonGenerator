using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door {
    public List<Vector2Int> TilePositions { get; private set; }
    public Vector2Int Size { get; private set; }

    public Door(List<Vector2Int> tilePositions) {
        if(tilePositions == null || tilePositions.Count == 0) {
            throw new System.ArgumentException("TilePositions cannot be null or empty", nameof(tilePositions));
        }

        TilePositions = tilePositions;
        Size = CalculateSize();
    }

    private Vector2Int CalculateSize() {
        var xValues = TilePositions.Select(tilePosition => tilePosition.x);
        var yValues = TilePositions.Select(tilePosition => tilePosition.y);

        return new Vector2Int(
            xValues.Max() - xValues.Min() + 1,
            yValues.Max() - yValues.Min() + 1
        );
    }
}
