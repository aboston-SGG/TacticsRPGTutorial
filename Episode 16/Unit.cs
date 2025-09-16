using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    public Vector2Int gridPosition;
    public float moveSpeed = 5f;
    public bool isMoving = false;
    private Vector3 targetPosition;

    private const float STOPPING_DISTANCE = 0.01f;

    public UnitStats stats;

    public int movementRange = 3;
    public int attackRange = 1;
    public int movementLeft;

    public Player owner;

    public List<Tile> path;

    public int maxHealth;
    public int health;

    public int attackDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = new UnitStats(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100));
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        attackRange = Mathf.Clamp(attackRange, 1, int.MaxValue);
        movementLeft = movementRange;
        maxHealth = 1 + Mathf.RoundToInt(stats.endurance * 0.25f);
        health = maxHealth;
        attackDamage = 1 + Mathf.RoundToInt(stats.strength * 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    // Initialize the movement values, called when we want to move our unit
    public void MoveTo(List<Tile> newPath)
    {
        if (!owner.isPlayerTurn || newPath == null || newPath.Count  == 0) return;

        Tile startTile = owner.gridManager.GetTile(gridPosition);
        startTile.isOccupied = false;
        Tile destinationTile = newPath[newPath.Count - 1];
        destinationTile.isOccupied = true;

        path = new List<Tile>(newPath);

        targetPosition = path[0].transform.position;
        gridPosition = path[0].gridPosition;
        isMoving = true;
    }

    // Handles the actual movement of the unit, called every frame
    private void HandleMovement()
    {
        if (!isMoving || path == null || path.Count == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if((transform.position - targetPosition).sqrMagnitude < STOPPING_DISTANCE * STOPPING_DISTANCE)
        {
            transform.position = targetPosition;

            movementLeft -= path[0].moveCost;

            path.RemoveAt(0);

            if(path.Count > 0)
            {
                targetPosition = path[0].transform.position;
                gridPosition = path[0].gridPosition;
            }
            else
            {
                isMoving = false;
                owner.ChangeSelectedUnit(this);
            }
        }
    }

    // Called when the mouse is clicked, changes the selected unit to this unit
    public void OnPointerDown(PointerEventData eventData)
    {
        if (owner.isPlayerTurn)
        {
            owner.ChangeSelectedUnit(this);
            return;
        }

        Unit playerUnit = Player.selectedUnit;

        if (playerUnit == null || playerUnit.isMoving) return;

        Tile unitTile = owner.gridManager.GetTile(gridPosition);
        Tile attackerTile = owner.gridManager.GetTile(playerUnit.gridPosition);

        int distance = owner.gridManager.GetHeuristic(unitTile, attackerTile);

        if (distance > playerUnit.attackRange) return;

        Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, playerUnit);

        if(closestAttackTile == null)
        {
            playerUnit.Attack(this);
            return;
        }

        List<Tile> path = owner.gridManager.GetPath(attackerTile, closestAttackTile);

        if(path == null || path.Count == 0)
        {
            playerUnit.Attack(this);
        }
        else
        {
            playerUnit.MoveTo(path);
        }
    }

    // Adds health
    public void Heal(int amount)
    {
        health += amount;
        if(health > maxHealth) health = maxHealth;
    }

    // Removes health
    public void TakeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            owner.playerUnits.Remove(this);
            Destroy(gameObject);
        }
    }

    // Deals damage to target
    public void Attack(Unit target)
    {
        Tile attackerTile = owner.gridManager.GetTile(gridPosition);
        Tile targetTile = owner.gridManager.GetTile(target.gridPosition);

        if(owner.gridManager.GetHeuristic(attackerTile, targetTile) <= attackRange)
        {
            target.TakeDamage(attackDamage);
        }
    }
}