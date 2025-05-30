using System.Collections.Generic;
using UnityEngine;

public class Door {
    public List<Vector2Int> TilePositions { get; }

    public Door(List<Vector2Int> tilePositions) {
        if(tilePositions == null || tilePositions.Count == 0) {
            throw new System.ArgumentException("TilePositions cannot be null or empty", nameof(tilePositions));
        }

        TilePositions = tilePositions;
    }
}
