# Random Dungeon Generator

> **Work in Progress** – This project is actively being developed. Features may be incomplete or subject to change.

A 2D procedural dungeon game built in Unity. The goal is to create a fully explorable dungeon with dynamic room layouts, enemies, weapons, and atmospheric effects.

## Implemented so far

All features listed below are in an early, rudimentary state and still being iterated on.

- **Procedural dungeon generation** – Rooms are placed using a queue-based BFS algorithm; each room picks a random size from a configurable list, with AABB overlap detection to prevent collisions
- **Variable room sizes** – Multiple room sizes can be defined in the Inspector; the generator picks randomly, and distribution bias controls how early outward spread kicks in
- **Floor & wall rendering** – Rooms are drawn with separate tilemaps for walls (with collision) and floors (no collision, rendered beneath walls and fog)
- **Fog of war** – Unexplored rooms are hidden by a fog overlay; the camera background is automatically synced to the fog color so the void always matches
- **Animated doors** – Doors play an open/close animation (Animator-driven); horizontal doors are single-tile sprites, vertical doors consist of a master collider with animated child panels per tile row
- **Room progression** – The first room entered from any cleared room (or the spawn) is always a **Perk Room**. Entering it reveals siblings and assigns them random event types (Normal, Cursed, Empty). After a room is cleared, its unvisited neighbors reset and follow the same pattern when next entered
- **Perk rooms** – Always the first room entered from a cleared area; presents a perk trigger at the room center and starts the encounter after the player interacts with it
- **Enemy encounters** – Enemies spawn when the player activates a room; doors close until the encounter is cleared
- **Wave & survival encounters** – Wave spawns all enemies at once; Survival spawns enemies continuously for a set duration
- **Faction-based enemies** – Enemies are grouped by faction (Undead, Orc, Demon); active faction is determined by the current world
- **Difficulty scaling** – Enemy count, survival duration, and spawn interval all scale with the number of cleared rooms; rare enemy types become more likely over time
- **Throwable weapon** – The player throws a weapon on left click; it sticks into whatever it hits, follows enemies when embedded, and drops to the floor on enemy death so it can be picked up
- **Melee attack** – Right click performs a melee slash in the direction of the cursor; uses an arc-shaped hitbox with a taper and soft-edge visual effect
- **Enemy separation** – Enemies push each other apart while chasing the player to avoid stacking
- **Edit mode preview** – The dungeon can be previewed in the Unity Editor without entering Play mode

## Systems Overview

| System | Description |
|---|---|
| `RoomGenerator` | Procedurally places rooms on a grid using BFS with AABB overlap detection |
| `DoorGenerator` | Creates doors between adjacent rooms using stored adjacency pairs |
| `RoomDrawer` | Draws wall and floor tilemaps for all rooms; floor tiles are selected per-position using weighted random selection |
| `DoorDrawer` | Instantiates door prefabs at shared room edges; places tilemap tiles and animated panel prefabs for vertical (E/W) doors |
| `PlayerGenerator` | Spawns the player (center, random, or edge room) |
| `EnemyManager` | Spawns and tracks enemies per room; runs wave or survival encounter coroutines |
| `DifficultyManager` | Scales enemy count, spawn interval and survival duration by cleared room count; selects enemies by weighted rarity |
| `WorldManager` | Tracks the current world and level; maps world number to enemy faction |
| `PerkManager` | Spawns a perk selection trigger in bonus rooms; starts the encounter after the player interacts |
| `DoorManager` | Opens/closes doors based on encounter state |
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
- **EnemyData assets** – ScriptableObjects (Create → Enemies → EnemyData) assigned to `EnemyGenerator`; each defines faction, rarity, prefab, stats and spawn weight

## Project Structure

- `Core/`
  - `DungeonComposer.cs` — Orchestrates the full generation pipeline
  - `DungeonConfig.cs` — ScriptableObject with dungeon parameters
  - `DungeonGenerationContext.cs` — Shared data passed between pipeline tasks
- `Generators/`
  - `RoomGenerator.cs` — Procedurally places rooms using BFS; records adjacency pairs
  - `DoorGenerator.cs` — Creates doors between rooms using stored adjacency pairs
  - `PlayerGenerator.cs` — Spawns the player and initializes runtime managers
  - `EnemyGenerator.cs` — Sets up the enemy system with EnemyData assets
- `Drawers/`
  - `RoomDrawer.cs` — Draws wall and floor tilemaps for all rooms
  - `DoorDrawer.cs` — Instantiates door prefabs at shared room edges
  - `RoomTriggerDrawer.cs` — Places room activation triggers
- `Managers/`
  - `RoomManager.cs` — Tracks the player's current room and cleared rooms
  - `EnemyManager.cs` — Spawns and manages enemies per room encounter
  - `DifficultyManager.cs` — Scales difficulty based on cleared room count
  - `WorldManager.cs` — Tracks current world/level; determines active enemy faction
  - `PerkManager.cs` — Handles perk trigger placement in bonus rooms
  - `DoorManager.cs` — Opens and closes doors based on game state
  - `FogOfWarManager.cs` — Manages fog overlays; syncs camera background to fog color
- `Controller/`
  - `DoorController.cs` — Animator-based door open/close logic; manages BoxCollider2D and all child Animators
  - `EnemyController.cs` — Enemy movement (with separation steering), health, and death events
  - `PlayerController.cs` — WASD movement and animation
  - `WeaponController.cs` — Handles throw (LMB) and melee (RMB) attacks; manages ammo
  - `ThrowableProjectile.cs` — Projectile that sticks into surfaces and enemies; pickable by the player
  - `CameraController.cs` — Smooth camera follow
  - `PerkSelectionTrigger.cs` — Trigger placed in bonus rooms; starts encounter on player contact
- `Models/`
  - `Room.cs` — Room data: position, size, bounds, neighbors, doors
  - `Door.cs` — Door data: tile positions, connected rooms
  - `EnemyData.cs` — ScriptableObject defining enemy identity, faction, rarity, prefab and stats
  - `WeaponData.cs` — ScriptableObject defining weapon stats for throw and melee attacks
  - `WeightedFloorTile.cs` — Serializable tile + weight pair for weighted floor tile selection
- `Enums/`
  - `EnemyFaction.cs` — Undead, Orc, Demon
  - `EnemyRarity.cs` — Normal, Uncommon, Rare, Boss
  - `EncounterMode.cs` — Wave, Survival

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
