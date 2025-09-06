# Episode 5: Movement Range

In this video we use Dijkstra's algorithm to determine which tiles we can move to, and highlight them accordingly.

## Edited Scripts
- Tile.cs
  - Made selectedTile public.
  - Added inMoveRange variable.
  - OnPointerExit() now changes color based on if the tile is in the movement range or not.
  - Calls HighlightMoveRange() instead of MoveTo() when the tile is selected.
- Unit.cs
  - Added a movement range variable.
- GridManager.cs
  - Added GetTileNeighbors() to retrieve all neighbors of a tile.
  - Added IsDiagonal() to check if a tile is diagonal to another.
  - Added ResetGridHighlights() to remove any existing color changes from movement.
  - Added GetHighlightRange() to retrieve a list of tiles the unit can move to using Dijkstra's algorithm.
  - Added HighlightMoveRange() to highlight all tiles a unite can move to.
