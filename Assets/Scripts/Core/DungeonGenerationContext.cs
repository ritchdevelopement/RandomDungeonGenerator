using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerationContext {
    [Header("Dungeon Data")]
    public Dictionary<Vector2Int, Room> createdRooms = new();
    public List<Door> createdDoors = new();
    public Vector2Int roomSize;
    public int numberOfRooms;
    public int roomDistributionFactor;
    public GameObject dungeonGameObject;

    [Header("Player Data")]
    public Vector2 playerSpawnPosition;
    public Room playerSpawnRoom;

    [Header("Materials")]
    public PhysicsMaterial2D frictionlessMaterial;
}
