# Block Puzzle

**🎮 [Play in your browser](https://haticenurum.itch.io/block-puzzle)**

My first Unity project. A grid puzzle where you drag shapes onto an 8x8 board and clear full rows or columns to score. The game ends when none of your current shapes can fit anywhere.

I built this to get hands-on with Unity and C# game architecture, coming from a data science and AI background.

## How it plays

Three random shapes appear each round. Drag them onto the grid; once placed, they lock. Fill a full row or column and it clears. When no remaining shape fits anywhere on the board, it's game over.

## What I focused on

I wanted the code to be clean and easy to extend, not just working. A few decisions I'm happy with:

- **Shapes as data, not code.** Each block shape (`BlockShapeData`) is a ScriptableObject asset. To add a new shape I just create a new asset and set its cells, with no code changes. New shapes were literally a two-minute job once this was in place.

- **Decoupled scoring with events.** `GridManager` doesn't know the score system exists. It fires an `OnLinesCleared` event, and `ScoreManager` subscribes to it. This keeps the grid logic focused on the grid.

- **One job per script.** Grid state (`GridManager`), spawning (`BlockSpawner`), dragging (`DraggableBlock`), scoring, game over, and animation are each their own component.

- **Effects done in code.** Placement bounce, the counting score, and the line-clear flash are all coroutines, with no imported animations.

## Scripts

- `GridManager`: grid state, coordinate math, line clearing, "can this shape fit?" checks
- `BlockSpawner`: spawns three shapes per round, detects game over
- `DraggableBlock`: dragging and multi-cell placement validation
- `BlockShapeData`: ScriptableObject defining a shape's cells and color
- `ScoreManager`: listens for line-clear events, updates the score
- `GameOverManager`: game over panel and restart
- `BlockAnimator`: placement bounce

## Notes

This is my first Unity project. I used AI assistance for guidance and debugging while writing and understanding the systems myself. There's known technical debt (see `TODO.md`), for example the game-over check and object pooling could be improved.