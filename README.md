# Random Dungeon Generator

> **Work in Progress** – This project is actively being developed. Features may be incomplete or subject to change.

A 2D procedural dungeon generation game built in Unity. The goal is to create a fully explorable dungeon with dynamic room layouts, enemies, and atmospheric effects.

## Implemented so far

All features listed below are in an early, rudimentary state and still being iterated on.

- **Procedural dungeon generation** – Rooms are placed using a queue-based BFS algorithm; each room picks a random size from a configurable list, with AABB overlap detection to prevent collisions
- **Variable room sizes** – Multiple room sizes can be defined in the Inspector; the generator picks randomly, and distribution bias controls how early outward spread kicks in
- **Floor & wall rendering** – Rooms are drawn with separate tilemaps for walls (with collision) and floors (no collision, rendered beneath walls and fog)
- **Fog of war** – Unexplored rooms are hidden by a fog overlay; the camera background is automatically synced to the fog color so the void always matches
- **Animated doors** – Doors play an open/close animation (Animator-driven); horizontal doors are single-tile sprites, vertical doors consist of a master collider with animated child panels per tile row
- **Enemy encounters** – An enemy spawns in each room the player enters; doors close until the enemy is defeated
- **Edit mode preview** – The dungeon can be previewed in the Unity Editor without entering Play mode

## Systems Overview

| System | Description |
|---|---|
| `RoomGenerator` | Procedurally places rooms on a grid using BFS with AABB overlap detection |
| `DoorGenerator` | Creates doors between adjacent rooms using stored adjacency pairs |
| `RoomDrawer` | Draws wall and floor tilemaps for all rooms; floor tiles are selected per-position using weighted random selection |
| `DoorDrawer` | Instantiates door prefabs at shared room edges; places tilemap tiles and animated panel prefabs for vertical (E/W) doors |
| `PlayerGenerator` | Spawns the player (center, random, or edge room) |
| `EnemyManager` | Spawns and tracks enemies per room |
| `DoorManager` | Opens/closes doors based on game state |
| `FogOfWarManager` | Manages fog overlays; syncs camera background to fog color |
| `RoomManager` | Tracks the player's current room and cleared rooms |

## Configuration

Dungeon parameters are controlled via the `DungeonConfig` ScriptableObject:

- **Room sizes** – List of allowed room sizes; each room picks one randomly (all values must be odd)
- **Number of rooms** – Default 25
- **Distribution bias** – Float [0..1]; controls when outward spread kicks in relative to total room count (0 = always spread, 1 = never spread, default 0.25)

Per-component settings (set directly on the component in the Inspector):

- **Door width** – Configurable on `DoorGenerator`, default 4 tiles (capped to the smaller of the two connected rooms)
- **Wall tile sets / Weighted floor tiles** – Tile assets assigned on `RoomDrawer`; floor tiles use weighted random selection per position (e.g. 70% stone, 20% moss, 10% cracks)
- **Fog color** – Assigned on `FogOfWarManager`; automatically applied to the camera background

## Project Structure

- `Core/`
  - `DungeonComposer.cs` — Orchestrates the full generation pipeline
  - `DungeonConfig.cs` — ScriptableObject with dungeon parameters
  - `DungeonGenerationContext.cs` — Shared data passed between pipeline tasks
- `Generators/`
  - `RoomGenerator.cs` — Procedurally places rooms using BFS; records adjacency pairs
  - `DoorGenerator.cs` — Creates doors between rooms using stored adjacency pairs
  - `PlayerGenerator.cs` — Spawns the player and initializes runtime managers
  - `EnemyGenerator.cs` — Sets up the enemy system
- `Drawers/`
  - `RoomDrawer.cs` — Draws wall and floor tilemaps for all rooms
  - `DoorDrawer.cs` — Instantiates door prefabs at shared room edges
  - `RoomTriggerDrawer.cs` — Places room activation triggers
- `Managers/`
  - `RoomManager.cs` — Tracks the player's current room and cleared rooms
  - `EnemyManager.cs` — Spawns and manages enemies per room encounter
  - `DoorManager.cs` — Opens and closes doors based on game state
  - `FogOfWarManager.cs` — Manages fog overlays; syncs camera background to fog color
- `Controller/`
  - `DoorController.cs` — Animator-based door open/close logic; manages BoxCollider2D and all child Animators
  - `EnemyController.cs` — Enemy AI, health, and death events
  - `PlayerController.cs` — WASD movement and animation
  - `CameraController.cs` — Smooth camera follow
- `Models/`
  - `Room.cs` — Room data: position, size, bounds, neighbors, doors
  - `Door.cs` — Door data: tile positions, connected rooms
  - `WeightedFloorTile.cs` — Serializable tile + weight pair for weighted floor tile selection

## Setup

After cloning the repository, run the setup script **once**:

```sh
./setup.sh
```

This activates two Git hooks:
- **commit-msg** – enforces the [Conventional Commits](https://www.conventionalcommits.org/) format
- **pre-commit** – checks C# formatting via `dotnet format`

To fix formatting issues automatically before committing:
```sh
dotnet format whitespace Assembly-CSharp.csproj
```

## Requirements

- Unity 2022.3 or newer
