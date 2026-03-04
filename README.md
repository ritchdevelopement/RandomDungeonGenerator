# Random Dungeon Generator

> Work in progress — things break, change, and get rewritten regularly.

A 2D dungeon crawler built in Unity. Procedurally generated rooms, enemies, weapons, and a progression system built around perk selection. Still early, but the core loop is taking shape.

## What's in so far

**Dungeon generation**
Rooms are placed via BFS — each one picks a random size from a configurable list, with AABB overlap checks so nothing intersects. Distribution bias controls how aggressively the generator spreads outward before filling in gaps.

**Rendering & atmosphere**
Rooms get separate tilemaps for walls (with collision) and floors (weighted random tile selection per position). Unexplored rooms are hidden behind a fog overlay; the camera background syncs to the fog color automatically.

**Doors**
Animator-driven open/close animations. Horizontal doors are a single sprite; vertical doors use a master collider with animated panel children per tile row.

**Room progression**
The first room you enter from any cleared area is always a perk room — you pick a perk there before the encounter starts. The moment you step in, the sibling rooms (other neighbors of the room you came from) become visible and get assigned random event types. After clearing a room, that same pattern resets for the next set of neighbors.

**Combat**
Doors close when a room activates. Wave mode spawns everything at once, survival mode drips enemies in over a timer and waits for stragglers before opening the doors. Difficulty scales with cleared room count — more rooms cleared means more enemies, shorter spawn intervals, and a higher chance of rarer enemy types.

**Enemies**
Grouped by faction (Undead / Orc / Demon) based on the current world. Each enemy type has its own stats and spawn weight. Enemies push each other apart while chasing so they don't all clump into one sprite.

**Weapons**
Left click throws, right click melees. The thrown weapon sticks into whatever it hits — if that's an enemy, it follows them around and drops on death so you can pick it up. Melee uses a physics arc with a tapered shape and soft edges. Both attacks share the ammo pool.

## Key systems

| System | What it does |
|---|---|
| `RoomGenerator` | BFS room placement with AABB overlap detection |
| `RoomManager` | Tracks cleared rooms, assigns events to newly revealed neighbors |
| `EnemyManager` | Runs wave/survival encounters, spawns enemies, handles room clear |
| `DifficultyManager` | Scales encounter parameters by cleared room count |
| `WorldManager` | Current world/level → active enemy faction |
| `PerkManager` | Spawns the perk trigger in perk rooms |
| `WeaponController` | Throw + melee input, ammo management, arc mesh |
| `DoorManager` | Opens/closes doors based on encounter state |
| `FogOfWarManager` | Fog overlays, camera background sync |

## Configuration

`DungeonConfig` ScriptableObject controls dungeon layout:
- **Room sizes** — list of odd-numbered sizes, picked randomly per room
- **Room count** — default 25
- **Distribution bias** — `[0..1]`, how early the generator spreads outward (0 = always, 1 = never, default 0.25)

Other things set per-component in the Inspector:
- Door width on `DoorGenerator` (default 4, capped to the smaller room)
- Wall tiles and weighted floor tiles on `RoomDrawer`
- Fog color on `FogOfWarManager`
- `EnemyData` assets on `EnemyGenerator` — create via `Create → Enemies → EnemyData`

## Project structure

```
Core/         DungeonComposer, DungeonConfig, DungeonGenerationContext
Generators/   RoomGenerator, DoorGenerator, PlayerGenerator, EnemyGenerator
Drawers/      RoomDrawer, DoorDrawer, RoomTriggerDrawer
Managers/     RoomManager, EnemyManager, DifficultyManager, WorldManager,
              PerkManager, DoorManager, FogOfWarManager
Controllers/  PlayerController, EnemyController, DoorController,
              WeaponController, ThrowableProjectile, CameraController,
              PerkSelectionTrigger
Models/       Room, Door, EnemyData, WeaponData, WeightedFloorTile
Enums/        EnemyFaction, EnemyRarity, EncounterMode, RoomEventType
```

## Setup

Run once after cloning:

```sh
./setup.sh
```

Installs two git hooks — one enforces [Conventional Commits](https://www.conventionalcommits.org/), the other runs `dotnet format` on C# files before each commit.

To fix formatting manually:
```sh
dotnet format whitespace Assembly-CSharp.csproj
```

**Requires Unity 2022.3+**
