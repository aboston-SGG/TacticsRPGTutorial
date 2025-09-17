# Episode 17: Enemy Attack

In this video we expand our enemy AI by allowing it to attack with its units.

## Edited Scripts
- PlayerAI.cs
  - Modified to include the remaining attacks of the activeUnit.
  - Sets all ai-controlled units to be marked as AI.
  - Renamed HandleUnitMovement() to HandleUnitTurn().
- Unit.cs
  - Added attacksLeft to track if the unit has attacked or can attack.
  - Factored attacksLeft into attacking.
  - Added isAI bool to make sure the unit only gets selected if it's not ai.
- Player.cs
  - Now resets the attacksLeft of the units when the turn starts.