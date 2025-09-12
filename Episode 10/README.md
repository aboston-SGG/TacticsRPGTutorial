# Episode 10: Unit Stats

In this video we add a struct to store some stats that will change how effective our units will be.

## New Scripts
- UnitStats.cs
  - Stores stat values for a unit.

## Edited Scripts
- Unit.cs
  - Added attackRange to represent how far the unit can attack.
  - Added UnitStats to store the stats for a unit.
  - Used the UnitStats constructor to initialize the stats to random values.
  - Modified the movementRange and attackRange values to be based on stats.