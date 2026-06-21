# Product Requirements Document: Snow Boarder
**Version:** 1.0.1
**Changelog from v1.0:**
- Added: PvP (Player vs. Player) local multiplayer mode — two players on one keyboard, same PC.
- Added: Dual input scheme — Player 1 uses Arrow Keys; Player 2 uses WASD.
- Added: Split-screen camera system for two simultaneous players using Cinemachine.
- Added: PvP-specific HUD, win condition, and Score Summary screen.
- Added: Player 2 sprite tint to visually distinguish boarders.
- Added: Per-player HUD border color — P1 panel border = `#00AAFF` (blue), P2 panel border = `#FF6B35` (orange) — so players can instantly identify their HUD in split-screen.
- Updated: Main Menu to include a mode-select screen (Solo vs. PvP).
- Updated: File structure, scripts list, and Tags/Layers table to reflect multiplayer additions.
- Updated: Open Questions and Assumptions sections.

---

## Product Overview

**Product Vision:** A 2D arcade-style snowboarding game built in Unity where one or two players control snowboarders navigating downhill slopes, dodging obstacles, collecting items, and performing tricks to maximise their score — with a local PvP mode allowing two players to race head-to-head on the same keyboard.

**Target Users:**
- Primary: Casual PC gamers aged 13–30 who enjoy quick, skill-based arcade games, including couch co-op play.
- Secondary: Unity students or junior developers studying 2D game mechanics, split-screen cameras, physics, and UI design.

**Business Objectives:**
- Deliver a complete, playable Unity 2D game demonstrating physics, collision handling, scoring, and UI.
- Serve as a graded academic submission that earns full marks across all rubric categories.
- Provide a codebase clean enough to extend with optional features (extra levels, online multiplayer).

**Success Metrics:**
- All 100 rubric points achieved on submission.
- Stable frame rate ≥ 60 FPS on a mid-range PC (Intel i5 / GTX 1060 equivalent) in both Solo and PvP modes.
- Zero crash-to-desktop bugs in a 10-minute play session.
- Playtester can understand controls for both players within 30 seconds without reading a manual.

---

## User Personas

### Persona 1: Alex — The Casual Gamer
- **Demographics:** 17 years old, high-school student, moderate PC gaming experience.
- **Goals:** Jump in, have fun for 5–10 minutes, beat a personal high score.
- **Pain Points:** Complex control schemes, unclear objectives, zero feedback on actions.
- **User Journey:** Launches game → reads main menu → clicks Start → plays Level 1 → crashes or finishes → sees score screen → restarts or quits.

### Persona 2: Jordan — The Unity Student
- **Demographics:** 21 years old, university game-dev course, intermediate Unity knowledge.
- **Goals:** Study how physics, triggers, and scoring are wired together; use the project as a reference.
- **Pain Points:** Spaghetti code, missing comments, non-standard folder structure.
- **User Journey:** Opens Unity Editor → browses Scripts folder → reads comments → runs Play Mode → inspects Inspector values → modifies parameters for personal learning.

### Persona 3: Sam & Chris — The Couch Competitors
- **Demographics:** 16–22 years old; two friends sharing one PC keyboard.
- **Goals:** Race each other down the slope; brag about winning; replay immediately.
- **Pain Points:** Controls overlap and trigger both characters; split-screen HUD is too small to read; no clear winner declaration.
- **User Journey:** Main menu → select **PvP Mode** → control reminder overlay shown for 3 s → Level 1 loads in split-screen → both race to the finish line → winner screen declares first-to-finish → Rematch or Main Menu.

---

## Feature Requirements

| Feature | Description | User Story | Priority | Acceptance Criteria | Dependencies |
|---------|-------------|------------|----------|---------------------|--------------|
| **Player Movement** | Left/right rotation and forward thrust via keyboard input. | As a player, I want to steer the snowboarder with arrow keys so I can navigate the slope. | Must | Arrow Left rotates CCW; Arrow Right rotates CW; Up applies forward torque; character never clips through terrain collider. | Rigidbody2D, slope tilemap |
| **Slope Physics** | Realistic gravity, friction, and momentum on angled terrain. | As a player, I want the boarder to accelerate downhill and slow on flat ground so it feels real. | Must | Boarder accelerates on ≥15° slopes; decelerates to stop on 0° flat; `PhysicsMaterial2D` friction value = 0.4 on snow tiles; mass = 1 kg. | Unity Physics 2D, Snow Profile asset |
| **Obstacle Course** | Rocks, trees, ramps, and jump zones placed in Level 1. | As a player, I want obstacles to challenge me so each run feels different. | Must | ≥5 rock obstacles, ≥3 tree obstacles, ≥2 ramp objects in Level 1; each has a `PolygonCollider2D` matching sprite shape. | Snow-Rock.png, Snow-Rock-2.png, Snow-Tree-1.png, Snow-Tree-2.png |
| **Snowflake Collection** | Collectible items scattered along the slope. | As a player, I want to collect snowflakes so I earn bonus points. | Must | ≥10 snowflakes in Level 1; each uses a `CircleCollider2D` set to `IsTrigger = true`; collected snowflake awards +50 points and plays a pickup sound; snowflake GameObject is destroyed on trigger enter. | Snowflake sprite (see Asset Sources), ScoreManager.cs |
| **Trick System** | Detect airtime and award trick points. | As a player, I want to perform tricks in the air to earn combo multipliers. | Must | Airtime ≥0.5 s triggers "Small Jump" trick (+100 pts); ≥1.0 s triggers "Big Air" (+300 pts); consecutive tricks without crashing increment a ×2 / ×3 / ×4 multiplier. | Rigidbody2D, TrickManager.cs |
| **Combo Multiplier** | Chain multiplier for consecutive successful tricks. | As a player, I want a multiplier to reward continuous good play. | Must | Multiplier resets to ×1 on crash; displayed in HUD; multiplier cap = ×4. | TrickManager.cs, HUD Canvas |
| **Crash Detection** | Colliding head-first with an obstacle triggers a crash. | As a player, I want crashes to feel impactful so I am penalised for mistakes. | Must | Crash is triggered when `Boarder_Top` collider contacts any obstacle tagged `"Obstacle"`; plays `Crash SFX.ogg`; deducts 1 life; respawns at last checkpoint or level start. | Boarder_Top.png collider, CrashHandler.cs, Crash SFX.ogg |
| **Lives System** | Player starts with 3 lives; game over at 0. | As a player, I want a lives system so crashes carry real consequences. | Must | Starting lives = 3; displayed as heart icons in HUD; game over scene shown when lives = 0. | HUD Canvas, LivesManager.cs |
| **Finish Line Trigger** | Trigger zone at the bottom of the slope ends the level. | As a player, I want a clear finish so I know when I have completed a run. | Must | `BoxCollider2D` with `IsTrigger = true` at slope end; plays `Finish SFX.ogg`; shows Score Summary screen. | Finish SFX.ogg, FinishLine.cs |
| **Speed Boost Power-Up** | Trigger zone that multiplies player speed for 3 s. | As a player, I want speed boosts so I can achieve higher scores. | Should | Power-up zone uses yellow star sprite; on trigger enter applies `velocity *= 1.5f` for 3 s; visual tint flashes during boost. | PowerUp.cs |
| **Invincibility Power-Up** | Trigger zone that ignores crash collisions for 5 s. | As a player, I want brief invincibility so I can take risks. | Should | Shield icon overlays boarder sprite; crash collider temporarily disabled for 5 s. | PowerUp.cs |
| **Main Menu UI** | Scene with Start, Options, Quit. | As a player, I want a main menu so I can start or exit the game cleanly. | Must | Three buttons functional; Start loads `Level1.unity`; Options opens audio-volume slider panel; Quit calls `Application.Quit()`. | MainMenu.unity (new scene), MenuManager.cs |
| **In-Game HUD** | Overlay showing score, speed, lives, multiplier. | As a player, I want real-time feedback so I know my performance. | Must | Score (top-left), speed in km/h (top-centre), lives as icons (top-right), multiplier badge (bottom-left); all update every frame. | HUDManager.cs, Unity UI (TextMeshPro) |
| **Score Summary Screen** | Post-run screen showing final score, best score, replay/quit. | As a player, I want to see my score after a run so I can aim to beat it. | Must | Displays current score, all-time best score (saved via `PlayerPrefs`); Replay and Main Menu buttons. | ScoreSummary.unity (new scene) |
| **Dynamic Snow Particles** | Falling snow particle system in the background. | As a player, I want a snowy atmosphere so the world feels immersive. | Should | Unity Particle System; emission rate = 80/s; lifetime = 4 s; gravity modifier = 0.1; uses white circle sprite 8×8 px. | Unity Particle System |
| **Background Clouds** | Scrolling cloud sprites in the background. | As a player, I want visual depth so the world feels alive. | Should | Three cloud sprites (`Sprite_Cloud_2.png`, `Sprite_Cloud_5.png`, `Sprite_Cloud_6.png`) on a separate background layer; auto-scroll left at 20 px/s; loop seamlessly. | CloudScroller.cs |
| **Camera Follow** | Smooth camera that trails the boarder down the slope. | As a player, I want the camera to follow me so I can see ahead. | Must | `Cinemachine 2D` virtual camera with `Body = Framing Transposer`; dead zone X = 0.1, Y = 0.2; look-ahead time = 0.5 s. | Cinemachine package |
| **Audio Manager** | Centralised volume control for SFX and music. | As a player, I want to adjust sound so I can play comfortably. | Should | `AudioManager.cs` singleton; exposes `SFXVolume` and `MusicVolume` (0–1); settings persisted via `PlayerPrefs`. | AudioManager.cs |
| **Mode Select Screen** | UI screen between Main Menu and gameplay letting players choose Solo or PvP. | As a player, I want to choose my game mode before starting so I am not forced into a mode. | Must | Mode Select shows two buttons: **Solo** and **PvP**; Solo loads `Level1.unity` with single-player setup; PvP loads `Level1_PvP.unity` with split-screen setup; Back button returns to Main Menu. | ModeSelectManager.cs, MainMenu.unity |
| **Player 2 Input (WASD)** | Second player controls mapped to WASD keys. | As Player 2, I want WASD controls so I can play simultaneously with Player 1 on the same keyboard. | Must | W = forward thrust; A = rotate CCW; D = rotate CW; no conflict with Arrow Keys at any point; both inputs polled in the same `FixedUpdate` via separate `PlayerInputData` structs. | PlayerController.cs (refactored), InputConfig.cs |
| **Player 2 Prefab** | Separate prefab for Player 2 with distinct visual identity. | As a player, I want to tell the two boarders apart so I always know which one I control. | Must | Player 2 uses the same `Boarder_Top.png` and `Boarder_Bottom.png` sprites but with a `Color` tint of `#FF6B35` (orange) applied to both `SpriteRenderer` components; tag = `Player2`; spawns at `SpawnPoint_P2` (x offset = +2 units from `SpawnPoint_P1`). | Player2.prefab, PlayerController.cs |
| **Split-Screen Camera** | Two independent Cinemachine virtual cameras, each tracking one player, rendered to separate viewports. | As a player, I want my own camera view so I can focus on my boarder without following the other player. | Must | Main Camera renders to `Rect(0, 0, 0.5, 1)` (left half); Camera2 renders to `Rect(0.5, 0, 0.5, 1)` (right half); each camera has its own `CinemachineVirtualCamera` with `Follow` set to its respective player; divider line sprite (1 px wide, full height, white) drawn at x = screen centre on a `UI` canvas. | Camera2 GameObject, SplitScreenDivider.cs, Cinemachine |
| **PvP Win Condition** | First player to cross the finish line wins the round. | As a player, I want a winner declared immediately when someone finishes so the result is unambiguous. | Must | `FinishLine.cs` records which player tag (`Player` or `Player2`) triggers it first; stores winner in `PvPGameManager.winner` (string); immediately freezes both players (`Rigidbody2D.simulated = false`); loads `PvPSummary.unity` within 0.5 s. | PvPGameManager.cs, FinishLine.cs |
| **PvP Lives & Crash (Per Player)** | Each player has independent lives; crashing only affects that player. | As a player, I want my crash not to affect my opponent so the game is fair. | Must | Each player has its own `LivesManager` instance with `startingLives = 3`; crash of Player 1 does not modify Player 2 lives and vice versa; if one player reaches 0 lives, that player is eliminated and the other wins immediately (same win flow as finish line). | LivesManager.cs (non-singleton), PvPGameManager.cs |
| **PvP HUD (Split-Screen)** | Each half of the screen shows the HUD for its respective player, with a colored border to match the player's identity. | As a player, I want my score and lives visible in my half of the screen with a clear color-coded border. | Must | Player 1 HUD panel has a **blue (#00AAFF)** border (2 px `Outline` component or background Image); Player 2 HUD panel has an **orange (#FF6B35)** border; font size minimum 16 pt (reduced from 18 pt to fit narrower viewport); each HUD shows: player label ("P1" / "P2"), score, speed (km/h), lives (heart icons), multiplier. | HUDManager_PvP.cs, Canvas per camera |
| **PvP Score Summary Screen** | Post-match screen showing winner, both players' scores, and replay option. | As a player, I want to see who won and both scores so I know by how much. | Must | Displays "Player 1 Wins!" or "Player 2 Wins!" in 48 pt font; shows P1 score and P2 score side-by-side; Rematch button reloads `Level1_PvP.unity`; Main Menu button loads `MainMenu.unity`. | PvPSummary.unity, PvPSummaryManager.cs |
| **Controls Reminder Overlay** | 3-second overlay at the start of PvP showing both control schemes. | As a new player, I want to see the controls before the run starts so I do not waste lives figuring them out. | Must | Full-screen semi-transparent black overlay; left half shows "P1: ← → ↑ to steer"; right half shows "P2: A D W to steer"; auto-dismisses after 3 s or on any key press; `Time.timeScale = 0` during overlay so physics do not start early. | ControlsOverlay.cs |

---

## User Flows

### Flow 1: Start a New Game
1. Player launches the executable → `MainMenu.unity` loads.
2. Player clicks **Start** → `SceneManager.LoadScene("Level1")`.
3. Level 1 loads; boarder spawns at top of slope; HUD initialises (Score=0, Lives=3, Multiplier=×1, Speed=0).
4. Player uses **Arrow Keys** to steer downhill.
   - Alternative: Player opens **Options** in main menu → adjusts volume → returns to menu.
   - Error state: Scene fails to load → Unity logs error; fallback shows "Level not found" text.

### Flow 2: Crash and Respawn
1. `Boarder_Top` collider contacts an `Obstacle`-tagged object.
2. `CrashHandler.OnCollisionEnter2D` fires → `Crash SFX.ogg` plays.
3. Lives decremented by 1 (HUD updates).
4. If Lives > 0: boarder position reset to `RespawnPoint` Transform; velocity zeroed; 1.5 s invincibility grace period.
5. If Lives = 0: `SceneManager.LoadScene("ScoreSummary")`.
   - Error state: `RespawnPoint` not assigned in Inspector → Debug.LogError fires; boarder teleports to (0, 0, 0) as fallback.

### Flow 3: Complete a Level
1. Boarder's collider enters the Finish Line `BoxCollider2D` trigger.
2. `FinishLine.OnTriggerEnter2D` → `Finish SFX.ogg` plays → time-freeze (`Time.timeScale = 0`).
3. `ScoreSummary` scene loads; final score and best score displayed.
4. Player clicks **Replay** → `Level1` reloads; or clicks **Main Menu** → `MainMenu` loads.

### Flow 4: Collect a Snowflake
1. Boarder trigger collider overlaps snowflake `CircleCollider2D`.
2. `PickupHandler.OnTriggerEnter2D` fires → `+50` added to `ScoreManager.currentScore`.
3. Snowflake GameObject destroyed; pickup particle burst plays (8 white sparks, lifetime 0.3 s).
4. HUD score value refreshes.

### Flow 5: Perform a Trick
1. Boarder leaves terrain (detected by `GroundChecker.cs` raycast returning false).
2. Airtime timer starts.
3. On landing (raycast returns true):
   - <0.5 s: no trick awarded.
   - 0.5–0.99 s: "Small Jump!" toast appears; +100 pts; multiplier incremented by 1 (cap ×4).
   - ≥1.0 s: "Big Air!" toast appears; +300 pts; multiplier incremented by 1 (cap ×4).
4. If next action is a crash: multiplier resets to ×1.

### Flow 6: Start a PvP Game
1. Player 1 launches the executable → `MainMenu.unity` loads.
2. Player 1 clicks **Start** → `ModeSelect.unity` loads.
3. Player 1 clicks **PvP** → `Level1_PvP.unity` loads.
4. Controls Reminder Overlay appears for 3 s (`Time.timeScale = 0`); overlay text:
   - Left panel: "P1 Controls: ← Rotate Left | → Rotate Right | ↑ Thrust"
   - Right panel: "P2 Controls: A Rotate Left | D Rotate Right | W Thrust"
5. Overlay dismisses (auto or any key) → `Time.timeScale = 1`; both boarders spawn and physics begin.
6. Both players race downhill simultaneously.
   - Error state: Player 2 GameObject not found in scene → `PvPGameManager.Awake()` logs `Debug.LogError("Player2 prefab not instantiated")`; game falls back to Solo mode automatically.

### Flow 7: PvP Win / Elimination
1. **Path A — Finish Line:** Player crosses `FinishLine` trigger → `FinishLine.OnTriggerEnter2D` records winner tag → `Finish SFX.ogg` plays → `Rigidbody2D.simulated = false` on both players → 0.5 s delay → `PvPSummary.unity` loads.
2. **Path B — Lives Eliminated:** One player reaches 0 lives → `PvPGameManager.OnPlayerEliminated(playerTag)` fires → opposing player is declared winner → same 0.5 s delay → `PvPSummary.unity` loads.
3. `PvPSummary.unity` shows winner banner, both scores.
4. **Rematch** → `Level1_PvP.unity` reloads (scores reset, lives reset to 3).
5. **Main Menu** → `MainMenu.unity` loads.
   - Error state: Winner tag is null (both players finish simultaneously, i.e., same frame) → `PvPGameManager` declares "Draw!" and shows both scores equal.

### Flow 8: PvP Crash (One Player)
1. Player 1's `Boarder_Top` collider contacts an `Obstacle`-tagged object.
2. `CrashHandler.OnCollisionEnter2D` fires for Player 1 only → `Crash SFX.ogg` plays.
3. Player 1's lives decremented by 1 (Player 1's HUD updates; Player 2's HUD is unaffected).
4. If Player 1 lives > 0: Player 1 respawns at `SpawnPoint_P1`; 1.5 s invincibility grace period. Player 2 continues uninterrupted.
5. If Player 1 lives = 0: `PvPGameManager.OnPlayerEliminated("Player")` → Player 2 wins (see Flow 7 Path B).

### Performance
- **Target Frame Rate:** ≥60 FPS on PC with Intel Core i5-8400 / GTX 1060 6 GB / 8 GB RAM.
- **Scene Load Time:** Level 1 fully loaded within 3 seconds on target hardware.
- **Physics Update Rate:** Fixed Timestep = 0.02 s (50 Hz) in Unity Project Settings > Time.
- **Draw Calls:** ≤50 draw calls per frame (use Sprite Atlas for all sprites).

### Security
- Local single-player game; no network calls required.
- High score stored via `PlayerPrefs` (key: `"HighScore"`, type: int). No external database.

### Compatibility
- **Operating System:** Windows 10 64-bit and Windows 11 64-bit.
- **Unity Version:** Unity 2022.3 LTS (exact version to avoid API incompatibilities).
- **Minimum Resolution:** 1280×720; UI uses Canvas Scaler set to `Scale With Screen Size`, reference = 1920×1080. Split-screen requires minimum 1280 px width so each viewport is ≥640 px wide.
- **Input — Solo Mode:** Keyboard only (Arrow Keys for steering; Escape for pause).
- **Input — PvP Mode:** Keyboard only; Player 1 = Arrow Keys; Player 2 = WASD; both schemes active simultaneously; no gamepad support in v1.0.1.
- **Keyboard Anti-Ghosting Note:** PvP requires simultaneous key presses (up to 6 keys at once). Standard membrane keyboards may ghost. Document this limitation in the in-game Options screen with the text: "PvP mode works best on a mechanical or anti-ghosting keyboard."

### Accessibility
- **Font Size:** Minimum 18 pt for all HUD text (TextMeshPro).
- **Colour Contrast:** All UI text passes WCAG 2.1 AA (contrast ratio ≥ 4.5:1 against background).
- **Pause Functionality:** Pressing Escape at any time freezes the game and shows a Pause Menu with Resume and Quit buttons.

---

## Technical Specifications

### File Structure (Unity Project)
```
Assets/
├── Audio/
│   ├── Crash SFX.ogg           ← provided
│   └── Finish SFX.ogg          ← provided
│   ├── pickup.ogg              ← download (see Asset Sources)
│   └── bgm_snow.ogg            ← download (see Asset Sources)
├── Scenes/
│   ├── MainMenu.unity          ← create
│   ├── ModeSelect.unity        ← create (NEW v1.0.1)
│   ├── Level1.unity            ← provided (extend for Solo)
│   ├── Level1_PvP.unity        ← create (NEW v1.0.1; duplicate of Level1 + split-screen setup)
│   ├── ScoreSummary.unity      ← create (Solo)
│   └── PvPSummary.unity        ← create (NEW v1.0.1)
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs     ← refactored: accepts PlayerInputData struct
│   │   ├── InputConfig.cs          ← NEW v1.0.1: defines key bindings per player ID
│   │   ├── GroundChecker.cs
│   │   └── CrashHandler.cs
│   ├── Managers/
│   │   ├── ScoreManager.cs         ← refactored: non-singleton, one instance per player
│   │   ├── LivesManager.cs         ← refactored: non-singleton, one instance per player
│   │   ├── TrickManager.cs
│   │   ├── AudioManager.cs
│   │   ├── GameManager.cs          ← Solo mode only
│   │   └── PvPGameManager.cs       ← NEW v1.0.1: owns PvP state, win/elimination logic
│   ├── UI/
│   │   ├── HUDManager.cs           ← Solo HUD
│   │   ├── HUDManager_PvP.cs       ← NEW v1.0.1: manages both P1 and P2 HUD panels
│   │   ├── MainMenuManager.cs
│   │   ├── ModeSelectManager.cs    ← NEW v1.0.1
│   │   ├── ControlsOverlay.cs      ← NEW v1.0.1
│   │   ├── ScoreSummaryManager.cs
│   │   └── PvPSummaryManager.cs    ← NEW v1.0.1
│   ├── Environment/
│   │   ├── CloudScroller.cs
│   │   ├── PickupHandler.cs
│   │   ├── PowerUp.cs
│   │   ├── FinishLine.cs           ← updated: detects which player tag crosses first
│   │   └── SplitScreenDivider.cs   ← NEW v1.0.1
├── Sprites/
│   ├── Characters/
│   │   ├── Boarder_Bottom.png  ← provided
│   │   └── Boarder_Top.png     ← provided
│   ├── Environment/
│   │   ├── Snow-Rock.png       ← provided
│   │   ├── Snow-Rock-2.png     ← provided
│   │   ├── Snow-tile-low-res.png ← provided
│   │   ├── Snow-Tree-1.png     ← provided
│   │   └── Snow-Tree-2.png     ← provided
│   ├── Clouds/
│   │   ├── Sprite_Cloud_2.png  ← provided
│   │   ├── Sprite_Cloud_5.png  ← provided
│   │   └── Sprite_Cloud_6.png  ← provided
│   ├── UI/
│   │   ├── heart_icon.png      ← see Asset Sources
│   │   ├── snowflake.png       ← see Asset Sources
│   │   ├── star_boost.png      ← see Asset Sources
│   │   └── shield_icon.png     ← see Asset Sources
│   └── SpriteAtlas.spriteatlas ← create in Unity
├── Tilemaps/
│   ├── SnowTilePalette.asset   ← create from Snow-tile-low-res.png
│   └── Snow Profile.asset      ← provided
├── Physics/
│   └── SnowMaterial.physicsMaterial2D ← create; Friction=0.4, Bounciness=0
└── Prefabs/
    ├── Player1.prefab              ← renamed from Player.prefab; tint = white (#FFFFFF)
    ├── Player2.prefab              ← NEW v1.0.1; same sprites, tint = #FF6B35 (orange)
    ├── Rock_Small.prefab
    ├── Rock_Large.prefab
    ├── Tree.prefab
    ├── Snowflake.prefab
    ├── PowerUp_Speed.prefab
    └── PowerUp_Shield.prefab
```

### Frontend (Unity 2D)
- **Engine:** Unity 2022.3 LTS
- **Render Pipeline:** Universal Render Pipeline (URP) 2D Renderer
- **UI System:** Unity UI with TextMeshPro (install via Package Manager: `com.unity.textmeshpro`)
- **Camera:** Cinemachine 2D (install via Package Manager: `com.unity.cinemachine`)
- **Tilemap:** Unity Tilemaps (built-in 2D package) for slope terrain using `Snow-tile-low-res.png`
- **Sprite Atlas:** One atlas at `Assets/Sprites/SpriteAtlas.spriteatlas` containing all sprites to reduce draw calls
- **Sorting Layers (back to front):**
  1. `Background` (sky gradient, clouds)
  2. `Midground` (distant trees, snow particles)
  3. `Environment` (slope tiles, obstacles)
  4. `Player`
  5. `Pickups`
  6. `UI`

### Backend / Game Logic
- **Language:** C# (Unity scripting)
- **No external backend** — all state is in-memory during a session; high score persisted via `PlayerPrefs`.
- **ScoreManager.cs:** Singleton; exposes `AddScore(int pts)`, `ApplyMultiplier()`, `ResetScore()`. Score = Σ(base_points × current_multiplier).
- **GameManager.cs:** Singleton; owns game-state enum (`MainMenu`, `Playing`, `Paused`, `GameOver`); exposes `PauseGame()`, `ResumeGame()`, `GameOver()`.

### Physics Setup
| Parameter | Value | Location |
|-----------|-------|----------|
| Gravity Scale (global) | -9.81 | Edit > Project Settings > Physics 2D |
| Player Rigidbody2D Mass | 1 kg | Player.prefab > Rigidbody2D |
| Player Angular Drag | 5 | Player.prefab > Rigidbody2D |
| SnowMaterial Friction | 0.4 | Assets/Physics/SnowMaterial.physicsMaterial2D |
| SnowMaterial Bounciness | 0 | Assets/Physics/SnowMaterial.physicsMaterial2D |
| Fixed Timestep | 0.02 s | Project Settings > Time |

### Input Configuration — `InputConfig.cs`

`InputConfig.cs` is a ScriptableObject at `Assets/Scripts/Player/InputConfig.asset`. Each `PlayerController.cs` is assigned one `InputConfig` instance in the Inspector. This decouples key bindings from logic.

```csharp
[CreateAssetMenu(fileName = "InputConfig", menuName = "SnowBoarder/InputConfig")]
public class InputConfig : ScriptableObject {
    public KeyCode rotateLeft;   // P1 = KeyCode.LeftArrow  | P2 = KeyCode.A
    public KeyCode rotateRight;  // P1 = KeyCode.RightArrow | P2 = KeyCode.D
    public KeyCode thrust;       // P1 = KeyCode.UpArrow    | P2 = KeyCode.W
}
```

Two asset instances are created in Unity:
- `Assets/Scripts/Player/InputConfig_P1.asset`: `rotateLeft = LeftArrow`, `rotateRight = RightArrow`, `thrust = UpArrow`
- `Assets/Scripts/Player/InputConfig_P2.asset`: `rotateLeft = A`, `rotateRight = D`, `thrust = W`

Assign `InputConfig_P1.asset` to `Player1.prefab > PlayerController > inputConfig` field.
Assign `InputConfig_P2.asset` to `Player2.prefab > PlayerController > inputConfig` field.

### Player Controller — Full Control Mapping

| Player | Input Key | Action | `PlayerController.cs` Call |
|--------|-----------|--------|---------------------------|
| P1 | Arrow Left | Rotate CCW | `rb.AddTorque(rotationSpeed)` |
| P1 | Arrow Right | Rotate CW | `rb.AddTorque(-rotationSpeed)` |
| P1 | Arrow Up | Forward thrust | `rb.AddForce(transform.up * thrustForce)` |
| P2 | A | Rotate CCW | `rb.AddTorque(rotationSpeed)` |
| P2 | D | Rotate CW | `rb.AddTorque(-rotationSpeed)` |
| P2 | W | Forward thrust | `rb.AddForce(transform.up * thrustForce)` |
| Either | Escape | Toggle pause | `GameManager.PauseGame()` (Solo) / `PvPGameManager.PauseGame()` (PvP) |

`rotationSpeed` = 200f (serialised in Inspector, same value for both players); `thrustForce` = 5f.

### Tags and Layers
| Tag | Applied To |
|-----|-----------|
| `Player` | Player 1 GameObject |
| `Player2` | Player 2 GameObject (NEW v1.0.1) |
| `Obstacle` | All rock and tree GameObjects |
| `Pickup` | Snowflake GameObjects |
| `PowerUp` | Speed boost and shield zones |
| `FinishLine` | Finish trigger collider |

### Split-Screen Camera Setup (NEW v1.0.1)

**Step-by-step setup in Unity Editor for `Level1_PvP.unity`:**

1. **Main Camera** (already in scene):
   - Set `Viewport Rect` → `X=0, Y=0, W=0.5, H=1` (left half of screen).
   - Add `CinemachineBrain` component.
   - Create child `CinemachineVirtualCamera` named `VCam_P1`:
     - `Follow` → drag `Player1` GameObject.
     - `Body` → `Framing Transposer`; `Dead Zone Width = 0.1`, `Dead Zone Height = 0.2`, `Look Ahead Time = 0.5`.
     - `Priority = 10`.

2. **Camera2** (new GameObject):
   - Add `Camera` component.
   - Set `Depth = 1` (renders on top of Main Camera, different viewport).
   - Set `Viewport Rect` → `X=0.5, Y=0, W=0.5, H=1` (right half of screen).
   - Set `Clear Flags = Depth Only` (shares skybox with Main Camera, no duplicate background draw).
   - Add `CinemachineBrain` component.
   - Create child `CinemachineVirtualCamera` named `VCam_P2`:
     - `Follow` → drag `Player2` GameObject.
     - `Body` → `Framing Transposer`; `Dead Zone Width = 0.1`, `Dead Zone Height = 0.2`, `Look Ahead Time = 0.5`.
     - `Priority = 10`.

3. **Divider Line** (UI):
   - Create `Canvas` with `Render Mode = Screen Space — Overlay`.
   - Add `Image` child named `SplitLineDivider`:
     - `Anchor` → stretch full height, fixed width.
     - `Width = 2 px`, `Height = 1080 px`.
     - `Pos X = 0` (screen centre).
     - `Color = #FFFFFF`, `Alpha = 0.8`.
   - Attach `SplitScreenDivider.cs` to keep the line centred on `Screen.width / 2` if resolution changes.

4. **HUD Canvases**:
   - `Canvas_HUD_P1`: `Render Mode = Screen Space — Camera`; `Render Camera = Main Camera`. All P1 HUD elements anchored to top-left.
   - `Canvas_HUD_P2`: `Render Mode = Screen Space — Camera`; `Render Camera = Camera2`. All P2 HUD elements anchored to top-left.
   - Do **not** use `Screen Space — Overlay` for PvP HUDs; overlaying canvases ignore viewport and will render to the full screen.

---

## Asset Sources

### Provided Assets (already in project)
All files under `Assets/Audio/` and `Assets/Sprites/` as listed in the file tree above.

### Missing Sprites — Free Download Sources

| Sprite | Filename | Source URL | Licence |
|--------|----------|------------|---------|
| Heart / Life icon | `heart_icon.png` | https://kenney.nl/assets/ui-pack (file: `heart.png` inside ZIP) | CC0 1.0 |
| Snowflake collectible | `snowflake.png` | https://kenney.nl/assets/particle-pack (file: `snow.png`) | CC0 1.0 |
| Speed boost star | `star_boost.png` | https://kenney.nl/assets/ui-pack (`star.png`) | CC0 1.0 |
| Shield icon | `shield_icon.png` | https://kenney.nl/assets/ui-pack (`shield.png`) | CC0 1.0 |
| Background sky gradient | `sky_bg.png` | https://opengameart.org/content/backgrounds-3 (file: `bg_sky.png`) | CC0 1.0 |

**Download instructions:**
1. Visit the URL above.
2. Click **Download** (no account required for Kenney assets).
3. Extract the ZIP.
4. Copy the specific file listed into `Assets/Sprites/UI/` (for UI sprites) or `Assets/Sprites/Environment/` (for world sprites).
5. In Unity, set **Texture Type = Sprite (2D and UI)**, **Pixels Per Unit = 100**, **Filter Mode = Point (no filter)** to match the provided pixel-art style.

### Free Unity Packages (Unity Asset Store — Free Tier)

| Package | Purpose | Asset Store URL | Package Name in Package Manager |
|---------|---------|----------------|--------------------------------|
| Cinemachine | Smooth camera follow | https://assetstore.unity.com/packages/essentials/cinemachine-79898 | `com.unity.cinemachine` |
| TextMeshPro | High-quality HUD text | Built into Unity 2022.3; install via Window > TextMeshPro > Import TMP Essential Resources | `com.unity.textmeshpro` |
| 2D Tilemap Extras | Extended tile brushes | https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest | `com.unity.2d.tilemap.extras` |

**Install via Package Manager:**  
`Window > Package Manager > Unity Registry` → search package name → click **Install**.

### AI Image Generation — Missing Sprites

If free assets above are unavailable or style does not match, generate replacements using the prompts below with **DALL·E 3** (https://chat.openai.com) or **Adobe Firefly** (https://firefly.adobe.com — free tier, 25 credits/month):

| Sprite Needed | Recommended Prompt |
|--------------|-------------------|
| Snowflake collectible (64×64 px) | `Pixel art snowflake icon, 64x64 pixels, white and light blue colours, black outline, transparent background, retro game style, no text` |
| Heart life icon (64×64 px) | `Pixel art heart icon, 64x64 pixels, bright red fill, dark red outline, transparent background, retro arcade game style, no text` |
| Speed boost star (64×64 px) | `Pixel art yellow star icon, 64x64 pixels, golden yellow colour, dark brown outline, transparent background, retro game power-up style, no text` |
| Shield icon (64×64 px) | `Pixel art shield icon, 64x64 pixels, blue and silver colours, black outline, transparent background, retro RPG game style, no text` |
| Sky background (1920×1080 px) | `2D game background, snowy mountain panorama, soft gradient sky from light blue to white, minimalist flat art style, no characters, no text, wide aspect ratio` |

**Post-generation steps:**
1. Download the generated image.
2. Open in **GIMP** (free: https://www.gimp.org) or **Paint.NET** (free: https://getpaint.net).
3. Export as PNG with transparent background (`File > Export As > .png`, check "Save background colour" OFF).
4. Resize to exact pixel dimensions listed above (`Image > Scale Image`).
5. Import into Unity as described in the Free Download Sources table above.

---

## Audio Requirements

| Sound | File | Trigger | Volume |
|-------|------|---------|--------|
| Crash effect | `Crash SFX.ogg` | `CrashHandler.OnCollisionEnter2D` on `Obstacle` tag | 0.8 |
| Level complete | `Finish SFX.ogg` | `FinishLine.OnTriggerEnter2D` | 1.0 |
| Snowflake pickup | `pickup.ogg` | `PickupHandler.OnTriggerEnter2D` on `Pickup` tag | 0.7 |
| Background music | `bgm_snow.ogg` | `AudioManager.Awake()`, looped | 0.5 |

**Free background music source:**  
- URL: https://opengameart.org/content/winter-chip  
- File: `winter_chip.ogg`  
- Licence: CC0 1.0  
- Rename to `bgm_snow.ogg` and place in `Assets/Audio/`.

**Free pickup SFX source:**  
- URL: https://kenney.nl/assets/interface-sounds (file: `confirmation_002.ogg`)  
- Rename to `pickup.ogg` and place in `Assets/Audio/`.

---

## Analytics & Monitoring

*(Local game — no remote analytics. Metrics tracked in-session via GameManager / PvPGameManager.)*

| Metric | Mode | Storage Method | Key |
|--------|------|---------------|-----|
| All-time high score | Solo | `PlayerPrefs.SetInt` | `"HighScore"` |
| Last session score | Solo | `PlayerPrefs.SetInt` | `"LastScore"` |
| Total solo runs | Solo | `PlayerPrefs.SetInt` | `"TotalRuns"` |
| P1 wins (lifetime) | PvP | `PlayerPrefs.SetInt` | `"PvP_P1Wins"` |
| P2 wins (lifetime) | PvP | `PlayerPrefs.SetInt` | `"PvP_P2Wins"` |
| Total PvP matches | PvP | `PlayerPrefs.SetInt` | `"PvP_TotalMatches"` |

---

## Release Planning

### v1.0 — Solo MVP (completed)
All features listed in the original v1.0 PRD.

### v1.0.1 — PvP Update (current)
**Features added:**
- Mode Select screen (Solo / PvP).
- Player 2 input scheme: WASD keys.
- Player 2 prefab with orange (#FF6B35) sprite tint.
- `InputConfig.cs` ScriptableObject for decoupled key bindings.
- Refactored `ScoreManager` and `LivesManager` to non-singleton (one instance per player).
- Split-screen camera: Main Camera (left) + Camera2 (right), each with dedicated `CinemachineVirtualCamera`.
- `HUDManager_PvP.cs` with per-player HUD canvases.
- `PvPGameManager.cs` managing win/elimination logic.
- PvP win conditions: first to finish line OR opponent runs out of lives.
- Controls Reminder Overlay (3 s, skippable).
- `PvPSummary.unity` scene with winner banner and Rematch button.
- `PvP_P1Wins`, `PvP_P2Wins`, `PvP_TotalMatches` tracked in `PlayerPrefs`.

**Timeline:** Complete alongside v1.0 submission or as immediate follow-up patch.

**Success Criteria:**
- Both players can complete a full PvP run without one player's input affecting the other.
- Split-screen renders correctly at 1920×1080 and 1280×720 without UI overflow.
- Winner is declared within 0.5 s of the finish line trigger.
- No frame rate drop below 60 FPS with both players simulated simultaneously.

### v1.1 — Optional Extensions
- Second mountain level with increased obstacle density.
- Time trial mode (Solo).
- 3 selectable snowboarder characters with different `mass` and `rotationSpeed`.

### v2.0 — Future Scope
- Procedurally generated slopes.
- Local leaderboard (JSON file at `Application.persistentDataPath`).
- Customisable snowboard skins (sprite swap).
- Online PvP via Unity Netcode for GameObjects.

---

## Open Questions & Assumptions

## Open Questions & Assumptions

**Open Questions:**
- Q1: Should the camera follow the Y-axis (downhill) only, or also follow X-axis rotation? — Decide before implementing `CinemachineVirtualCamera` settings.
- Q2: Is a restart checkpoint mid-level required, or always respawn at the top? — Current assumption: respawn at top.
- Q3: Does the slope in `Level1.unity` already have a `Tilemap Collider 2D`, or does it need to be added? — Verify on first scene open.
- Q4 (NEW): Should snowflake pickups in PvP be shared (first player to touch gets them) or per-player (each player has their own set)? — Current assumption: **shared** — first player to touch the snowflake gets the points and destroys it; opponent cannot collect the same one.
- Q5 (NEW): In PvP, if both players crash and reach 0 lives simultaneously, who wins? — Current assumption: **Draw** declared; `PvPGameManager` checks for simultaneous elimination in the same frame and sets `winner = "Draw"`.
- Q6 (NEW): Does the obstacle layout in `Level1_PvP.unity` need to be different from `Level1.unity` to accommodate two players side-by-side, or is the existing slope width sufficient? — Verify by playtesting; if player paths overlap on narrow sections, widen the spawn separation.

**Assumptions:**
- A1: Unity 2022.3 LTS is installed on the development machine. Do not use Unity 6.x (API differences).
- A2: `Boarder_Top.png` represents the head/upper body; used as crash-detection collider. `Boarder_Bottom.png` is the board/lower body for terrain contact. Both assumptions apply to both Player 1 and Player 2 prefabs.
- A3: `Snow Profile.asset` is a `PhysicsMaterial2D`; apply it to the tilemap collider for snow friction.
- A4: All provided sprites import correctly. Verify with `Texture Type = Sprite (2D and UI)` in Inspector.
- A5: Game runs in a fixed 1920×1080 window; no fullscreen toggle required for MVP.
- A6 (NEW): `ScoreManager` and `LivesManager` are **not** singletons in v1.0.1. Each player prefab carries its own instance. Code that previously called `ScoreManager.Instance.AddScore()` must be refactored to pass a reference to the correct instance (resolved via `GetComponentInParent<ScoreManager>()` in `PickupHandler.cs`).
- A7 (NEW): Player 2's orange tint (`#FF6B35`) is applied at runtime in `Player2.prefab` via `SpriteRenderer.color`; no additional sprite files are required — the existing `Boarder_Top.png` and `Boarder_Bottom.png` are reused.
- A8 (NEW): The PvP level (`Level1_PvP.unity`) is a **duplicate** of `Level1.unity` (File > Save As). Split-screen camera GameObjects and dual spawn points are added without modifying the original Level1 scene.
- A9 (NEW): Per-player HUD **border color** is applied to the HUD panel `Image` component's `color` property (or an `Outline` UI Effect): `#00AAFF` for P1, `#FF6B35` for P2. `HUDManager_PvP.cs` sets this color in `Awake()` based on which player the HUD instance belongs to. No additional sprites are needed.

---

## Appendix

### Competitive Analysis

| Game | Strengths | Weaknesses | Lessons for Snow Boarder |
|------|-----------|-----------|--------------------------|
| **Alto's Adventure** | Fluid one-touch controls, gorgeous visuals, endless runner loop | Mobile-first (portrait); paid | Use simple 2-key input; prioritise feel over complexity |
| **SkiFree (1991)** | Iconic, surprising enemy (Yeti), accessible | No scoring system, dated graphics | Add clear scoring and feedback loops |
| **Snowboard Kids (N64)** | Trick system, character variety | Complex 3D controls | Simplified trick system via airtime detection is sufficient |

### Glossary

| Term | Definition |
|------|-----------|
| **Rigidbody2D** | Unity component that gives a GameObject mass and makes it react to physics forces. |
| **PhysicsMaterial2D** | Unity asset defining friction and bounciness of a surface; assigned to a Collider2D. |
| **IsTrigger** | Collider2D property: when true, the collider detects overlaps but does not physically block objects. |
| **Sorting Layer** | Unity system for controlling the render order of 2D sprites. |
| **Cinemachine** | Unity package providing advanced camera behaviours (follow, damping, look-ahead). |
| **PlayerPrefs** | Unity API for persisting small amounts of data (int, float, string) between play sessions. |
| **Sprite Atlas** | Unity asset that packs multiple sprites into one texture to reduce draw calls. |
| **CC0 1.0** | Creative Commons Zero licence — fully public domain, no attribution required. |
| **TextMeshPro** | Unity text rendering system with higher quality than legacy UI Text; supports rich text tags. |
| **Fixed Timestep** | The interval at which Unity's physics engine updates; default 0.02 s = 50 physics steps per second. |