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
When you step into a room, the other exits from where you came from become visible and get assigned a random event type — wave or survival. Perk rooms pause the game and show three perk choices before the encounter starts. The same thing repeats every time you move deeper into the dungeon.

**Perks**
After clearing a perk room you pick one of three randomly drawn perks: extra max health (also fills the new heart), more ammo, faster dash, higher move speed, or piercing shots that pass through enemies.

**Combat**
Doors close when a room activates. Wave mode spawns everything at once, survival mode drips enemies in over a timer and waits for stragglers before opening the doors. Difficulty scales with cleared room count — more rooms cleared means more enemies, shorter spawn intervals, and a higher chance of rarer enemy types.

**Enemies**
Grouped by faction (Undead / Orc / Demon) based on the current world. Each enemy type has its own stats and spawn weight. Enemies push each other apart while chasing so they don't all clump into one sprite.

**Player**
WASD to move, space to dash. Dashing makes you invulnerable and lets you pass through enemies without taking contact damage. Health, ammo, and dash cooldown are shown in the HUD.

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
| `PerkManager` | Shows perk selection UI, applies chosen perk |
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
  - `RoomManager.cs` — Tracks cleared rooms, assigns events to newly revealed neighbors
  - `EnemyManager.cs` — Spawns and manages enemies per room encounter
  - `DifficultyManager.cs` — Scales difficulty based on cleared room count
  - `WorldManager.cs` — Tracks current world/level; determines active enemy faction
  - `PerkManager.cs` — Spawns the perk trigger in perk rooms
  - `DoorManager.cs` — Opens and closes doors based on encounter state
  - `FogOfWarManager.cs` — Manages fog overlays; syncs camera background to fog color
- `Controllers/`
  - `PlayerController.cs` — WASD movement and animation
  - `EnemyController.cs` — Enemy movement (with separation steering), health, and death events
  - `DoorController.cs` — Animator-based door open/close logic
  - `WeaponController.cs` — Throw (LMB) and melee (RMB) attacks; ammo management
  - `ThrowableProjectile.cs` — Sticks into surfaces and enemies; pickable by the player
  - `CameraController.cs` — Smooth camera follow
  - `PerkOptionTrigger.cs` — Spawned in perk rooms; opens perk selection UI on player contact
- `Models/`
  - `Room.cs` — Room data: position, size, bounds, neighbors, doors
  - `Door.cs` — Door data: tile positions, connected rooms
  - `EnemyData.cs` — ScriptableObject: faction, rarity, prefab, stats, spawn weight
  - `WeaponData.cs` — ScriptableObject: throw and melee stats
  - `PerkData.cs` — ScriptableObject: perk type, value, icon, name, description
  - `WeightedFloorTile.cs` — Tile + weight pair for weighted floor tile selection
- `Enums/`
  - `EnemyFaction.cs` — Undead, Orc, Demon
  - `EnemyRarity.cs` — Normal, Uncommon, Rare, Boss
  - `EncounterMode.cs` — Wave, Survival
  - `RoomEventType.cs` — Normal, Empty, Cursed, Perk
  - `PerkType.cs` — MaxHealth, MaxAmmo, MoveSpeed, DashCooldown, Piercing, ...

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

**Requires Unity 6000.3.10f1**
