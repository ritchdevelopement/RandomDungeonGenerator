using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : DungeonTaskBase {
    private List<RectInt> reservedBounds = new();

    public override void Execute() {
        reservedBounds.Clear();
        CreateRooms();

        Debug.Log($"Generated {context.createdRooms.Count} rooms out of requested {context.numberOfRooms}.");
    }

    private void CreateRooms() {
        Vector2Int firstSize = PickRandomSize();
        Room firstRoom = new Room(firstSize, Vector2Int.zero);

        Queue<Room> roomsToCreate = new();
        roomsToCreate.Enqueue(firstRoom);
        reservedBounds.Add(firstRoom.Bounds);

        while (roomsToCreate.Count > 0 && context.createdRooms.Count < context.numberOfRooms) {
            Room currentRoom = roomsToCreate.Dequeue();
            context.createdRooms[currentRoom.Center] = currentRoom;
            AddNeighbours(currentRoom, roomsToCreate);
        }
    }

    private void AddNeighbours(Room currentRoom, Queue<Room> roomsToCreate) {
        Direction[] directions = { Direction.North, Direction.South, Direction.East, Direction.West };
        List<(Direction Dir, Room Candidate)> available = new();

        foreach (Direction dir in directions) {
            Vector2Int candidateSize = Room.EnsureOddSize(PickRandomSize());
            Vector2Int candidateCenter = currentRoom.GetNeighborCenter(dir, candidateSize);

            if (context.createdRooms.ContainsKey(candidateCenter)) {
                continue;
            }

            RectInt candidateBounds = new RectInt(
                candidateCenter.x - candidateSize.x / 2,
                candidateCenter.y - candidateSize.y / 2,
                candidateSize.x,
                candidateSize.y
            );

            if (OverlapsAnyReserved(candidateBounds)) {
                continue;
            }

            available.Add((dir, new Room(candidateSize, candidateCenter)));
        }

        if (context.createdRooms.Count > context.roomDistributionFactor) {
            RemoveClosestToCenter(available);
        }

        int maxNeighbours = Random.Range(1, available.Count + 1);

        for (int i = 0; i < maxNeighbours && available.Count > 0; i++) {
            int index = Random.Range(0, available.Count);
            (Direction Dir, Room Candidate) chosen = available[index];
            available.RemoveAt(index);

            roomsToCreate.Enqueue(chosen.Candidate);
            reservedBounds.Add(chosen.Candidate.Bounds);
            context.adjacencies.Add((currentRoom, chosen.Candidate, chosen.Dir));

            available.RemoveAll(entry => OverlapsAnyReserved(entry.Candidate.Bounds));
        }
    }

    private bool OverlapsAnyReserved(RectInt candidate) {
        foreach (RectInt reserved in reservedBounds) {
            if (candidate.Overlaps(reserved)) {
                return true;
            }
        }
        return false;
    }

    private void RemoveClosestToCenter(List<(Direction Dir, Room Candidate)> available) {
        if (available.Count == 0) {
            return;
        }

        (Direction Dir, Room Candidate) closest = available
            .OrderBy(entry => Vector2Int.Distance(entry.Candidate.Center, Vector2Int.zero))
            .First();

        available.Remove(closest);
    }

    private Vector2Int PickRandomSize() {
        if (context.roomSizes == null || context.roomSizes.Count == 0) {
            throw new System.InvalidOperationException("DungeonConfig.roomSizes must contain at least one entry.");
        }
        return context.roomSizes[Random.Range(0, context.roomSizes.Count)];
    }

}
