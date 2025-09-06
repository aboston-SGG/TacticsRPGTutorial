# Episode 6: Unit Selection

In this video we adjust our code to allow us to select a unit and move it according to its movement range.

## New Scripts
- Player.cs
  - Handles logic for the player.
  - Tracks and handles unit selection changes.

## Edited Scripts
- Tile.cs
  - Extracted the SelectTile() method for cleanliness.
  - Added logic to move or deselect a unit when clicked.
- GridManager.cs
  - Added GetTile() to retrieve a tile using a Vector2Int.
- Unit.cs
  - Added OnPointerDown() to select the unit.
  - Added player ownership.
