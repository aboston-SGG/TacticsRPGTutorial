using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private Player player;
    private GridManager gridManager;
    public List<Unit> units;
    private Unit activeUnit;
    private Unit targetUnit;

    public enum AIBehaviors
    {
        Random,
        Aggressive,
        Defensive,
        Smart,
        Dumb
    }

    public AIBehaviors behavior = AIBehaviors.Random;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        gridManager = player.gridManager;
        units.AddRange(player.playerUnits);
        activeUnit = units[0];

        foreach(Unit unit in units)
        {
            unit.isAI = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isPlayerTurn)
        {
            CheckBehavior();
            HandleUnitTurn();
            CheckUnit();
        }
    }

    // Returns target based on health
    private Unit GetTargetByHealth(bool findLowest = true)
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        int health = findLowest ? int.MaxValue : int.MinValue;
        Unit targetUnit = null;

        foreach(Unit unit in enemyUnits)
        {
            if (unit.owner == player) continue;

            if(findLowest && unit.health < health)
            {
                health = unit.health;
                targetUnit = unit;
            }
            else if(!findLowest && unit.health > health)
            {
                health = unit.health;
                targetUnit = unit;
            }
        }

        return targetUnit;
    }

    // Returns target based on distance from the activeUnit
    private Unit GetTargetByDistance(Unit fromUnit, bool findClosest = true)
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        int distance = findClosest ? int.MaxValue : int.MinValue;
        Unit targetUnit = null;

        foreach (Unit unit in enemyUnits)
        {
            if (unit.owner == player) continue;

            int currentDistance = gridManager.GetHeuristic(gridManager.GetTile(fromUnit.gridPosition), gridManager.GetTile(unit.gridPosition));

            if (findClosest && distance > currentDistance)
            {
                distance = currentDistance;
                targetUnit = unit;
            }
            else if (!findClosest && distance < currentDistance)
            {
                distance = currentDistance;
                targetUnit = unit;
            }
        }

        return targetUnit;
    }

    // Returns a random target
    private Unit GetRandomTarget()
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        List<Unit> possibleTargets = new List<Unit>();

        foreach (Unit unit in enemyUnits)
        {
            if(unit.owner != player)
            {
                possibleTargets.Add(unit);
            }
        }

        if (possibleTargets.Count == 0) return null;
        int randomIndex = Random.Range(0, possibleTargets.Count);
        return possibleTargets[randomIndex];
    }

    // Finds the tile that's the furthest away from all enemies
    private Tile GetFurthestTileFromEnemy()
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        List<Unit> enemyList = new List<Unit>();

        foreach(Unit unit in enemyUnits)
        {
            if (unit.owner != player)
            {
                enemyList.Add(unit);
            }
        }

        int maxDistance = int.MinValue;
        Tile furthestTile = null;

        foreach(Tile tile in gridManager.map)
        {
            if (!tile.inMoveRange) continue;

            foreach(Unit enemy in enemyList)
            {
                int distance = gridManager.GetHeuristic(gridManager.GetTile(enemy.gridPosition), tile);

                if(distance > maxDistance)
                {
                    maxDistance = distance;
                    furthestTile = tile;
                }
            }
        }

        return furthestTile;

    }

    // Assigns AI behavior based on remaining units
    private void CheckBehavior()
    {
        int unitsLeft = player.playerUnits.Count;

        if(unitsLeft > 25)
        {
            behavior = AIBehaviors.Random;
        }
        else if(unitsLeft > 20)
        {
            behavior = AIBehaviors.Dumb;
        }
        else if(unitsLeft > 15)
        {
            behavior = AIBehaviors.Defensive;
        }
        else if(unitsLeft > 10)
        {
            behavior = AIBehaviors.Aggressive;
        }
        else
        {
            behavior = AIBehaviors.Smart;
        }
    }

    // Handles the turn for the activeUnit
    private void HandleUnitTurn()
    {
        if (activeUnit.isMoving) return;

        gridManager.GetHighlightRange(activeUnit.gridPosition, activeUnit.movementLeft, activeUnit.attackRange);

        Tile targetTile = null;

        switch (behavior)
        {
            case AIBehaviors.Random:
                targetUnit = GetRandomTarget();
                break;
            case AIBehaviors.Dumb:
                if(Random.value > 0.5f)
                {
                    targetUnit = GetTargetByHealth(false);
                }
                else
                {
                    targetUnit = GetTargetByDistance(activeUnit, false);
                }
                break;
            case AIBehaviors.Defensive:
                if(activeUnit.health < activeUnit.maxHealth * 0.3f)
                {
                    targetTile = GetFurthestTileFromEnemy();
                }
                else
                {
                    targetUnit = GetTargetByDistance(activeUnit);
                }
                break;
            case AIBehaviors.Aggressive:
                targetUnit = GetTargetByDistance(activeUnit);
                break;
            case AIBehaviors.Smart:
                targetUnit = GetTargetByHealth();
                break;
        }

        if(targetUnit == null)
        {
            activeUnit.movementLeft = 0;
            activeUnit.attacksLeft = 0;
            return;
        }

        int distance = gridManager.GetHeuristic(gridManager.GetTile(activeUnit.gridPosition), gridManager.GetTile(targetUnit.gridPosition));

        if(distance <= activeUnit.attackRange && activeUnit.attacksLeft > 0)
        {
            activeUnit.Attack(targetUnit);
            activeUnit.attacksLeft--;
            return;
        }
        else
        {
            activeUnit.attacksLeft = 0;
        }

        if (targetTile == null)
        {
            targetTile = gridManager.GetClosestAttackTile(targetUnit, activeUnit);
        }

        if (targetTile != null)
        {
            List<Tile> path = gridManager.GetPath(gridManager.GetTile(activeUnit.gridPosition), targetTile);

            if (path != null && path.Count > 0)
            {
                activeUnit.MoveTo(path);
            }
            else
            {
                activeUnit.movementLeft = 0;
            }
        }
        else
        {
            activeUnit.movementLeft = 0;
        }
    }

    // Checks if the activeUnit is expended and mvoes tot he next one
    private void CheckUnit()
    {
        gridManager.ResetGridHighlights();

        if(activeUnit.movementLeft <= 0 && activeUnit.attacksLeft <= 0)
        {
            targetUnit = null;
            units.Remove(activeUnit);

            if(units.Count > 0)
            {
                activeUnit = units[0];
            }
            else
            {
                player.readyToEndTurn = true;
            }
        }
    }
}
