using System;
using UnityEngine.Tilemaps;

[Serializable]
public class WeightedFloorTile {
    public TileBase tile;
    public float weight = 1f;
}
