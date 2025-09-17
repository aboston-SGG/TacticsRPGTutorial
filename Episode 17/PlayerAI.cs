using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private Player player;
    private GridManager gridManager;
    public List<Unit> units;
    private Unit activeUnit;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isPlayerTurn)
        {
            CheckBehavior();
            HandleUnitMovement();
            CheckUnit();
        }
    }

    // Returns a target based on the target's current health
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

    // Returns a target based on the target's distance from the activeUnit
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

    // Finds the tile with the greatest distance from any enemy
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

    // Moves the active unit
    private void HandleUnitMovement()
    {
        if (activeUnit.isMoving) return;

        gridManager.GetHighlightRange(activeUnit.gridPosition, activeUnit.movementLeft, activeUnit.attackRange);

        Tile targetTile = null;

        switch (behavior)
        {
            case AIBehaviors.Random:
                targetTile = gridManager.GetClosestAttackTile(GetRandomTarget(), activeUnit);
                break;
            case AIBehaviors.Dumb:
                if(Random.value > 0.5f)
                {
                    targetTile = gridManager.GetClosestAttackTile(GetTargetByHealth(false), activeUnit);
                }
                else
                {
                    targetTile = gridManager.GetClosestAttackTile(GetTargetByDistance(activeUnit, false), activeUnit);
                }
                break;
            case AIBehaviors.Defensive:
                if(activeUnit.health < activeUnit.maxHealth * 0.3f)
                {
                    targetTile = GetFurthestTileFromEnemy();
                }
                else
                {
                    targetTile = gridManager.GetClosestAttackTile(GetTargetByDistance(activeUnit), activeUnit);
                }
                break;
            case AIBehaviors.Aggressive:
                targetTile = gridManager.GetClosestAttackTile(GetTargetByDistance(activeUnit), activeUnit);
                break;
            case AIBehaviors.Smart:
                targetTile = gridManager.GetClosestAttackTile(GetTargetByHealth(), activeUnit);
                break;
        }

        if(targetTile != null)
        {
            List<Tile> path = gridManager.GetPath(gridManager.GetTile(activeUnit.gridPosition), targetTile);
            activeUnit.MoveTo(path);
        }
        else
        {
            activeUnit.movementLeft = 0;
        }
    }

    // Checks if the active unit is expended, then moves on to the next one
    private void CheckUnit()
    {
        gridManager.ResetGridHighlights();

        if(activeUnit.movementLeft <= 0)
        {
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
