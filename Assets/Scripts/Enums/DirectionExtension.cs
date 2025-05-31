using UnityEngine;

public static class DirectionExtensions {
    public static Direction Opposite(this Direction dir) => dir switch {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        _ => throw new System.ArgumentOutOfRangeException(nameof(dir), dir, null)
    };
}
