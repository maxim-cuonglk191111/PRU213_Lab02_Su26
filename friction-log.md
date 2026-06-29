# Friction Log

## 2026-06-13 — PRU222_Lab02_Su26 (Snow Boarder)
- **Sprites folder was flat (no subfolders)**: All sprites lived directly in `Assets/Sprites/` without the `Characters/`, `Environment/`, `Clouds/`, `UI/` subdirectory structure specified in the PRD; had to move them via PowerShell after creating directories.
- **Scripts folder was completely empty**: The Unity project had no C# scripts at all despite the PRD listing ~17 required scripts; all had to be created from scratch.
- **PRD missing explicit border-color feature**: PRD v1.0.1 mentioned Player 2 sprite *tint* (#FF6B35) but had no explicit per-player HUD *border color* feature; added it as A9 in Assumptions and updated the PvP HUD feature row before execution.
- **`Rigidbody2D.velocity` → `linearVelocity` in Unity 6**: Unity 2022.3 LTS still uses `.velocity`, but the warning about the renamed property (`linearVelocity`) in newer versions caused confusion; scripts use `linearVelocity` defensively since this project will stay on 2022.3 LTS.
- **`rembg` requires `[cpu]` or `[gpu]` extra**: Plain `pip install rembg` installs without an onnxruntime backend and crashes at runtime; must use `pip install "rembg[cpu]"` on CPU-only machines.
- **Editor scripts referencing runtime MonoBehaviours must compile first**: `SnowBoarderSetup_Part1/2.cs` in `Assets/Editor/` reference runtime scripts (e.g. `PlayerController`, `PowerUp`); Unity must compile the runtime assembly before the Editor scripts compile — open the project in Unity and wait for the initial compile cycle to finish before running the menu items.

## 2026-06-29 — Unity Obsolete API Churn (PRU222_Lab02_Su26)
- **`FindObjectOfType` → `FindFirstObjectByType` → `FindAnyObjectByType` in one version jump**: Unity 6 deprecated `FindObjectOfType`, the replacement `FindFirstObjectByType` was *also* immediately deprecated (instance-ID ordering issue), requiring a second pass to replace everything with `FindAnyObjectByType`; always use `FindAnyObjectByType` in Unity 6+.
- **`FindObjectsSortMode` enum removed in Unity 6**: The overload `FindObjectsByType<T>(FindObjectsSortMode)` was deprecated alongside the enum itself; use the `FindObjectsByType<T>(FindObjectsInactive)` overload instead (no sort mode parameter).
- **Unity `??` null-coalescing operator fails on `GetComponent<T>()`**: Unity overrides `==` for fake-null destroyed objects but does NOT override `??`, so `GetComponent<T>() ?? AddComponent<T>()` may return a "fake null" wrapper instead of adding the component — always use explicit `if (x == null) x = AddComponent<T>()` for Unity Objects.
- **Buttons in Main Menu not responding — Inspector refs null + EventSystem missing**: Editor setup scripts (Part 3) wired `Button` references via `SerializedObject`, but if the scene is re-opened or rebuilt the links can be lost; fix is to make Manager scripts self-heal by finding buttons by name in `Start()`, and to auto-create `EventSystem` at runtime if absent.

## 2026-06-29 — Lab 02 Full Audit (SnowBoarder)
- **`activeInputHandler: 1` (New Input System Only) kills ALL Legacy Input**: The project had `activeInputHandler: 1` but ALL gameplay code uses `UnityEngine.Input` (Legacy). `StandaloneInputModule` crashes immediately because `Input.mousePosition` throws `InvalidOperationException`. Fix: set to `2` (Both) in `ProjectSettings.asset`.
- **PlayerController had no InputConfig assigned — player never moves**: `SnowBoarderSetup_Part3.WirePlayerComponents()` never called `SetRef(so, "inputConfig", cfg)`. Must explicitly load `InputConfig_P1.asset` / `InputConfig_P2.asset` and assign via `SerializedObject` for each player.
- **No CameraFollow script existed anywhere**: Camera stayed at (0,0,-10) and never tracked the player. Created `CameraFollow.cs` and wired it to MainCamera in both Level1 and Level1_PvP via Part3 setup.
- **Summary screens (ScoreSummary, PvPSummary) had no self-heal fallback**: If Inspector refs were null (broken links), buttons silently did nothing. Added `FindBtn()`/`FindTMP()` fallbacks and runtime `EnsureEventSystem()` guards.
