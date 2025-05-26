using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerationContext {
    public Dictionary<Vector2Int, Room> createdRooms = new();
    public Vector2Int roomSize;
    public int numberOfRooms;
    public int roomDistributionFactor;
}
