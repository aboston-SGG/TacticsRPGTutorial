# Episode 12: Movement Limitation

In this video we limit our unit movement so they can't move infinitely in a turn.

## Edited Scripts
- Unit.cs
  - Added movementLeft variable to track how much the unit can move this turn.
  - Made isMoving public.
  - Modified HandleMovement() to update the movementLeft value as the unit moves along the path.
  - HandleMovement() now reselects the unit when it's done moving.
- Tile.cs
  - Modified OnPointerDown() to do nothing if the selected unit is currently moving.
- Player.cs
  - Updated the HighlightRange() call inside ChangeSelectedUnit() so that it takes the unit's movementLeft as an argument instead of movementRange.