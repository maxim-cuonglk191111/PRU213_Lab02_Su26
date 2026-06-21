# Friction Log

## 2026-06-13 — PRU222_Lab02_Su26 (Snow Boarder)
- **Sprites folder was flat (no subfolders)**: All sprites lived directly in `Assets/Sprites/` without the `Characters/`, `Environment/`, `Clouds/`, `UI/` subdirectory structure specified in the PRD; had to move them via PowerShell after creating directories.
- **Scripts folder was completely empty**: The Unity project had no C# scripts at all despite the PRD listing ~17 required scripts; all had to be created from scratch.
- **PRD missing explicit border-color feature**: PRD v1.0.1 mentioned Player 2 sprite *tint* (#FF6B35) but had no explicit per-player HUD *border color* feature; added it as A9 in Assumptions and updated the PvP HUD feature row before execution.
- **`Rigidbody2D.velocity` → `linearVelocity` in Unity 6**: Unity 2022.3 LTS still uses `.velocity`, but the warning about the renamed property (`linearVelocity`) in newer versions caused confusion; scripts use `linearVelocity` defensively since this project will stay on 2022.3 LTS.
- **`rembg` requires `[cpu]` or `[gpu]` extra**: Plain `pip install rembg` installs without an onnxruntime backend and crashes at runtime; must use `pip install "rembg[cpu]"` on CPU-only machines.
- **Editor scripts referencing runtime MonoBehaviours must compile first**: `SnowBoarderSetup_Part1/2.cs` in `Assets/Editor/` reference runtime scripts (e.g. `PlayerController`, `PowerUp`); Unity must compile the runtime assembly before the Editor scripts compile — open the project in Unity and wait for the initial compile cycle to finish before running the menu items.
