# Episode 11: Attack Range

In this video we add another pass of our Dijkstra's algorithm to display our attack range.

## Edited Scripts
- GridManager.cs
  - Expanded GetTileNeighbors() to also be able to exluded diagonal tiles.
  - Modified ResetGridHighlights() to factor in attack range.
  - Modified GetHighlightRange() to include attack range by adding a second pass of the algorithm.
  - Renamed HighlightMoveRange() to HighlightRange() and added logic to highlight attackable tiles.
- Tile.cs
  - Added inAttackRange bool to determine if the tile should be highlighted.
  - Modified OnPointerExit() to also factor in the attack range.
- Player.cs
  - Updated the HighlightRange() call inside ChangeSelectedUnit() so that it takes the unit's attack range as an argument.