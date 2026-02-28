using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerationContext {
    public Dictionary<Vector2Int, Room> createdRooms = new();
    public List<Door> createdDoors = new();
    public List<(Room RoomA, Room RoomB, Direction Dir)> adjacencies = new();
    public List<Vector2Int> roomSizes;
    public int numberOfRooms;
    public int roomDistributionFactor;
    public GameObject dungeonGameObject;
    public Vector2 playerSpawnPosition;
    public Room playerSpawnRoom;
    public PhysicsMaterial2D frictionlessMaterial;
}
