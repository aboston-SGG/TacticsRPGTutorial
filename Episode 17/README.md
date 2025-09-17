# Episode 17: Enemy Movement

In this video we start our enemy AI by allowing it to move units around on its turn.

## New Scripts
- PlayerAI.cs
  - Manages the automatic functions performed by non-human players.

## Edited Scripts
- Player.cs
  - Added the ability to track when the player is ready to end their turn.
- TurnManager.cs
  - Automatically ends the enemy turn when all units have been expended.