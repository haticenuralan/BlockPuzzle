# Block Puzzle

A grid-based puzzle game built with Unity as my first Unity project. Drag and drop multi-cell shapes onto an 8x8 grid; complete full rows or columns to clear them and earn points. The game ends when none of the available shapes can fit on the board.

## Gameplay

- **8x8 grid** with drag-and-drop block placement
- **Multiple block shapes** (single, lines, L-shape) defined as data assets
- **Line clearing** — completing a full row or column clears it with a flash effect
- **Scoring** with combo bonuses for clearing multiple lines at once
- **Game over detection** — the game ends only when no remaining shape can fit anywhere on the grid
- **Restart** to play again

## Technical Highlights

This project was built to demonstrate clean, maintainable Unity/C# code:

- **Data-driven design (ScriptableObject):** Block shapes are defined as `BlockShapeData` assets. Adding a new shape requires creating a new asset — no code changes needed.
- **Event-based decoupling (Observer pattern):** `GridManager` broadcasts an `OnLinesCleared` event instead of directly calling the score system. `ScoreManager` subscribes to it. The grid doesn't need to know the score system exists.
- **Single-responsibility components:** Grid logic (`GridManager`), block spawning (`BlockSpawner`), dragging (`DraggableBlock`), scoring (`ScoreManager`), game over (`GameOverManager`), and animation (`BlockAnimator`) are each isolated.
- **Coroutine-based juice:** Placement bounce, animated score counter, and line-clear flash effects are implemented with coroutines — no external animation assets.
- **Grid math abstraction:** World-to-grid and grid-to-world coordinate conversions are centralized in `GridManager`.

## Architecture Overview

- `GridManager` — grid state, coordinate conversion, line clearing, and "can this shape fit?" queries
- `BlockSpawner` — spawns three random shapes each round, checks for game over
- `DraggableBlock` — handles dragging and multi-cell snapping/placement validation
- `BlockShapeData` (ScriptableObject) — defines a shape's occupied cells and color
- `ScoreManager` — listens for line-clear events and updates the animated score UI
- `GameOverManager` — shows the game over panel and handles restart
- `BlockAnimator` — placement bounce effect

## Development Notes

This is my first Unity project, built to strengthen my game-development skills alongside my background in data science and AI. I used AI-assisted development for guidance, debugging, and boilerplate, while implementing and understanding every system myself.

## Known Limitations

See TODO.md for documented technical debt, including planned improvements like object pooling.
