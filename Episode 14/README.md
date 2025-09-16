# Episode 14: Auto-Positioning

In this video we make it so that our units can move to an ideal position to attack an enemy.

## Edited Scripts
- Player.cs
  - Made it so that units are automatically assigned the player as their owner.
  - Added a variable to show if it's the player's turn.
- TurnManager.cs
  - EndTurn() now updates isPlayerTurn.
- Unit.cs
  - Units only take actions when it is their owner's turn.
  - Added logic to move the selected unit to an ideal attack position when another unit is clicked.
- GridManager.cs
  - Added GetClosestAttackTile() to get a tile in the ideal attack range of an enemy.