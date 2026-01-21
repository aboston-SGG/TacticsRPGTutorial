using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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

    public int attacksLeft = 1;

    public bool isAI;

    public int level = 1;
    public float experience = 0f;
    public float experienceToLevel = 100f;

    public float physicalDefense;

    public Image healthBar;

    public UnitClass unitClass;
    public UnitStats characterGrowths;

    public string unitName;
    public Sprite unitPortrait;

    public Inventory inventory;

    public BaseWeapon equippedWeapon;

    public float critChance;
    public float critModifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeStats();
        UpdateStatValues();
        movementLeft = movementRange;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        healthBar.fillAmount = (float)health / (float)maxHealth;
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
                
                if(!isAI) owner.ChangeSelectedUnit(this);
            }
        }
    }

    // Called when the mouse is clicked, changes the selected unit to this unit
    public void OnPointerDown(PointerEventData eventData)
    {
        if (owner.isPlayerTurn && !IsUnitExpended())
        {
            owner.ChangeSelectedUnit(this);
            return;
        }

        Unit playerUnit = Player.selectedUnit;

        if (playerUnit == null || playerUnit.isMoving) return;

        Tile unitTile = owner.gridManager.GetTile(gridPosition);
        Tile attackerTile = owner.gridManager.GetTile(playerUnit.gridPosition);

        int distance = owner.gridManager.GetHeuristic(unitTile, attackerTile);

        if(distance <= playerUnit.attackRange)
        {
            if(playerUnit.attacksLeft > 0)
            {
                playerUnit.Attack(this);
            }
            else
            {
                return;
            }
        }

        Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, playerUnit);

        if(closestAttackTile == null)
        {
            return;
        }

        List<Tile> path = owner.gridManager.GetPath(attackerTile, closestAttackTile);

        if(path == null || path.Count == 0)
        {
            return;
        }
        
        playerUnit.MoveTo(path);
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

        int distance = owner.gridManager.GetHeuristic(attackerTile, targetTile);

        if (distance <= attackRange)
        {
            attacksLeft--;
            int damageToDeal = Mathf.RoundToInt(Mathf.Clamp(GetAttackDamage() - target.physicalDefense, 0, target.maxHealth));

            if (GetCriticalHit())
            {
                Debug.Log("Critical Hit!");
                damageToDeal = Mathf.RoundToInt(damageToDeal * critModifier);
            }

            float experienceToAdd = target.health <= damageToDeal ? damageToDeal * 10 : damageToDeal;
            experienceToAdd *= Mathf.Clamp(10 - (level - target.level), 0, 10);
            target.TakeDamage(damageToDeal);

            experience += experienceToAdd;

            if(experience >= experienceToLevel)
            {
                LevelUp();
            }
        }
    }

    // Check if the unit has been used up for the turn
    public bool IsUnitExpended()
    {
        if(movementLeft <= 0 && attacksLeft <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Updates the derived stat values based on the base stats
    private void UpdateStatValues()
    {
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        critChance = Mathf.Clamp01(stats.luck * 0.005f);

        if(equippedWeapon != null)
        {
            attackRange = Mathf.Clamp(attackRange, equippedWeapon.minRange, equippedWeapon.maxRange);
            critChance = Mathf.Clamp01(equippedWeapon.critChance + (stats.luck * 0.005f));
        }

        maxHealth = 1 + Mathf.RoundToInt(stats.endurance * 0.25f);
        physicalDefense = Mathf.RoundToInt((stats.endurance * 0.02f) + (stats.strength * 0.01f));
        critModifier = 1.1f + (stats.mastery * 0.01f);
    }

    // Levels up the unit
    private void LevelUp()
    {
        if (isAI) return;

        experience -= experienceToLevel;
        experienceToLevel *= 1.5f;
        level++;

        stats.LevelUpStats(unitClass.classGrowths, characterGrowths);
        UpdateStatValues();

        if(experience >= experienceToLevel)
        {
            LevelUp();
        }
    }

    // Initializes the unit's stats based on their class
    public void InitializeStats()
    {
        UnitStats baseClassStats = unitClass.baseStats;
        UnitStats growthStats = unitClass.classGrowths;

        stats = new UnitStats(
            UnitStats.InitializeStatValue(baseClassStats.speed, growthStats.speed),
            UnitStats.InitializeStatValue(baseClassStats.perception, growthStats.perception),
            UnitStats.InitializeStatValue(baseClassStats.endurance, growthStats.endurance),
            UnitStats.InitializeStatValue(baseClassStats.strength, growthStats.strength),
            UnitStats.InitializeStatValue(baseClassStats.luck, growthStats.luck),
            UnitStats.InitializeStatValue(baseClassStats.intellect, growthStats.intellect),
            UnitStats.InitializeStatValue(baseClassStats.spirit, growthStats.spirit),
            UnitStats.InitializeStatValue(baseClassStats.mastery, growthStats.mastery));
    }

    // Expend the unit for the turn
    public void ExpendUnit()
    {
        movementLeft = 0;
        attacksLeft = 0;
    }

    // Called when the mouse pointer enters the unit's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        Player.hoverUnit = this;
    }

    // Called when the mouse pointer exits the unit's area
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Player.hoverUnit == this)
        {
            Player.hoverUnit = null;
        }
    }

    // Calculates the attack damage of the unit
    private float GetAttackDamage()
    {
        return equippedWeapon 
            ? Mathf.RoundToInt(equippedWeapon.power * (1 + (stats.strength * 0.05f))) 
            : 1 + Mathf.RoundToInt(stats.strength * 0.05f);
    }

    // Changes the equipped weapon and updates stats accordingly
    public void ChangeEquippedWeapon(BaseWeapon weapon)
    {
        equippedWeapon = weapon;
        UpdateStatValues();
        owner.ChangeSelectedUnit(this);
    }

    // Determines if an attack critically hits
    private bool GetCriticalHit()
    {
        return Random.value < critChance;
    }
}