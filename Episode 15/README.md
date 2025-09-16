# Episode 15: Attacking

In this video we give our units the ability to attack, breaking our auto-positioning in the process.

## Edited Scripts
- GridManager.cs
  - Made GetHeuristic() public.
- Unit.cs
  - Added health and damage variables.
  - Updated Start() to initialize health and damage.
  - Added Heal() and TakeDamage() to handle health management.
  - Modified OnPointerDown() to trigger the attack from the selected unit to the clicked unit.
- UnitStats.cs
  - Added new stats and updated the constructor.