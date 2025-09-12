# Episode 9: A* Pathfinding

In this video use the A* pathfinding algorithm to make our units follow the shortest path to get to their desitination instead fo just gliding right to it.

## Edited Scripts
- Tile.cs
  - Added pathfinding variables.
- GridManager.cs
  - Added GetHeuristic() to find the Manhattan distance between two tiles.
  - Added RetracePath() to follow back the path created by the A* algorithm and make it usable.
  - Added GetPath() to use A* to find the shortest path to the target tile we want our unit to move to.
- Unit.cs
  - Added a list of tiles to hold the path our unit will follow.
  - Modified HandleMovement() to follow the path during movement.
