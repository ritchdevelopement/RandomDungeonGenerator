using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : DungeonTaskBase {
    private HashSet<Vector2Int> reservedPositions = new();

    public override void Execute() {
        reservedPositions.Clear();
        CreateRooms();

        Debug.Log($"Generated {context.createdRooms.Count} rooms out of requested {context.numberOfRooms}.");
    }

    private void CreateRooms() {
        Vector2Int initialRoomPos = Vector2Int.zero;

        Queue<Room> roomsToCreate = new();
        roomsToCreate.Enqueue(new Room(context.roomSize, initialRoomPos));
        reservedPositions.Add(initialRoomPos);

        while(roomsToCreate.Count > 0 && context.createdRooms.Count < context.numberOfRooms) {
            Room currentRoom = roomsToCreate.Dequeue();
            context.createdRooms[currentRoom.Center] = currentRoom;
            AddNeighbour(currentRoom, roomsToCreate);
        }
    }

    private void AddNeighbour(Room currentRoom, Queue<Room> roomsToCreate) {
        List<Vector2Int> neighbourPositions = currentRoom.GetNeighbourPositions();
        List<Vector2Int> availableNeighbours = new();

        foreach(Vector2Int position in neighbourPositions) {
            if(context.createdRooms.ContainsKey(position) || reservedPositions.Contains(position)) {
                continue;
            }

            availableNeighbours.Add(position);
        }

        if(context.createdRooms.Count > context.roomDistributionFactor) {
            RemoveClosestRoomToCenter(availableNeighbours);
        }

        int maxNumberOfNeighbors = Random.Range(1, availableNeighbours.Count + 1);

        for(int i = 0; i < maxNumberOfNeighbors && availableNeighbours.Count > 0; i++) {
            Vector2Int chosen = availableNeighbours[Random.Range(0, availableNeighbours.Count)];
            availableNeighbours.Remove(chosen);
            roomsToCreate.Enqueue(new Room(context.roomSize, chosen));
            reservedPositions.Add(chosen);
        }
    }

    private void RemoveClosestRoomToCenter(List<Vector2Int> availableNeighbours) {
        if(availableNeighbours.Count == 0) {
            return;
        }

        Vector2Int closest = availableNeighbours
            .OrderBy(pos => Vector2Int.Distance(pos, Vector2Int.zero))
            .First();

        availableNeighbours.Remove(closest);
    }
}
