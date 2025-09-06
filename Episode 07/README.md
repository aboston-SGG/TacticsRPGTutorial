# Episode 7: Terrain Costs

In this video we add terrain costs to our map generation and movement range.

## Edited Scripts
- Tile.cs
  - Added movement cost variable.
- GridManager.cs
  - Added the ability to take a string of numbers and pull tile movement cost from it.
  - Changed GenerateGrid() to change colors based on the tile movement cost mapped to a gradient.
