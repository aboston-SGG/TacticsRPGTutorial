# Episode 8: Tile Occupancy

In this video we add occupancy to our tiles so that our units won't be able to pile on top of each other.

## Edited Scripts
- Tile.cs
  - Added an occupancy variable to check if a tile has a unit on it or not.
  - Changes tile occupancy when moving units.
- GridManager.cs
  - Added GetTile() overload that accepts a world position as an argument.
  - Added occupancy check to the movement range algorithm.
  - Changed Start() to Awake() to maintain script execution order.
- Player.cs
  - Added a list of units the player owns.
  - Added SnapUnits() to snap the units to the grid.
