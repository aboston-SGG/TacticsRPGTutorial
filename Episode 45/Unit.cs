using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Unit : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Grid & Movement")]
    [Tooltip("Current grid position of the unit")]
    public Vector2Int gridPosition;
    [Tooltip("Movement Speed in units per second.")]
    public float moveSpeed = 5f;
    [Tooltip("Max Movement Range per turn.")]
    public int movementRange = 3;
    [Tooltip("Current Movement left this turn.")]
    public int movementLeft;
    [Tooltip("Base Attack Range of the unit, without a weapon equipped.")]
    public int attackRange = 1;
    [Tooltip("Is the unit currently moving along a path.")]
    public bool isMoving = false;
    private Vector3 targetPosition;
    private const float STOPPING_DISTANCE = 0.01f;
    [HideInInspector]public List<Tile> path;

    [Header("Ownership")]
    [Tooltip("The player or AI who controls this unit.")]
    public Player owner;

    [Header("Stats")]
    public UnitStats stats;
    [Tooltip("Max Health of the unit.")]
    public int maxHealth;
    [Tooltip("Current Health of the unit.")]
    public int health;
    [Tooltip("Chance to deal a critical hit.")]
    public float critChance;
    [Tooltip("Multiplier applied on critical damage.")]
    public float critModifier;
    [Tooltip("Chance to hit a target.")]
    public float accuracy;
    [Tooltip("Physical defense value, reduces damage from physical attacks.")]
    public float physicalDefense;

    [Header("Turn State")]
    [Tooltip("Number of attacks left this turn.")]
    public int attacksLeft = 1;
    [Tooltip("Is this unit controlled by AI?")]
    public bool isAI;

    [Header("Progression")]
    public int level = 1;
    public float experience = 0f;
    public float experienceToLevel = 100f;
    public UnitClass unitClass;
    public UnitStats characterGrowths;

    [Header("Appearance & UI")]
    public string unitName;
    public Sprite unitPortrait;
    public Image healthBar;

    [Header("Inventory & Equipment")]
    public Inventory inventory;
    public ItemInstance equippedWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeStats();
        UpdateStatValues();
        inventory.InitializeInventory();
        movementLeft = movementRange;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if(healthBar != null) healthBar.fillAmount = (float)health / (float)maxHealth;
    }

    #region Movement
    // Handles the actual movement of the unit, called every frame
    private void HandleMovement()
    {
        if (!isMoving || path == null || path.Count == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if ((transform.position - targetPosition).sqrMagnitude < STOPPING_DISTANCE * STOPPING_DISTANCE)
        {
            transform.position = targetPosition;

            movementLeft -= path[0].moveCost;

            path.RemoveAt(0);

            if (path.Count > 0)
            {
                targetPosition = path[0].transform.position;
                gridPosition = path[0].gridPosition;
            }
            else
            {
                isMoving = false;

                if (!isAI) owner.ChangeSelectedUnit(this);
            }
        }
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
    #endregion

    #region Combat
    // Adds health
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    // Removes health
    public void TakeDamage(int amount, Unit attacker)
    {
        health -= amount;
        if (health <= 0)
        {
            Die(attacker);
        }
    }

    // Deals damage to target
    public void Attack(Unit target)
    {
        if(target == null) return;

        Tile attackerTile = owner.gridManager.GetTile(gridPosition);
        Tile targetTile = owner.gridManager.GetTile(target.gridPosition);

        int distance = owner.gridManager.GetHeuristic(attackerTile, targetTile);

        if (distance > attackRange) return;
        if (attacksLeft <= 0) return;
        
        attacksLeft--;

        if (!GetAttackHit())
        {
            Debug.Log($"{unitName} missed {target.unitName}.");
            return;
        }

        BaseWeapon weaponData = equippedWeapon.definition as BaseWeapon;
        if (weaponData != null) equippedWeapon.currentDurability--;

        int damageToDeal = Mathf.RoundToInt(Mathf.Clamp(GetAttackDamage(target.equippedWeapon?.definition) - target.physicalDefense, 0, target.maxHealth));

        if (GetCriticalHit())
        {
            Debug.Log("Critical Hit!");
            damageToDeal = Mathf.RoundToInt(damageToDeal * critModifier);
        }

        float experienceToAdd = target.health <= damageToDeal ? damageToDeal * 10 : damageToDeal;
        experienceToAdd *= Mathf.Clamp(10 - (level - target.level), 0, 10);
        target.TakeDamage(damageToDeal, this);
        experience += experienceToAdd;

        if (experience >= experienceToLevel)
        {
            while (experience >= experienceToLevel) LevelUpOnce();
        }

        Debug.Log($"{unitName} dealt {damageToDeal} damage to {target.unitName}.");
    }

    // Calculates the attack damage of the unit
    private float GetAttackDamage(BaseItem defendingWeapon)
    {
        BaseWeapon equippedWeaponData = equippedWeapon.definition as BaseWeapon;

        if (equippedWeaponData == null) return 1 + Mathf.RoundToInt(stats.strength * 0.05f);

        BaseWeapon defendingWeaponData = defendingWeapon as BaseWeapon;
        WeaponType defendingType = defendingWeaponData != null ? defendingWeaponData.weaponType : WeaponType.None;

        float positiveModifier = equippedWeaponData.effectiveAgainst.HasFlag(defendingType) ? 1.5f : 1f;
        float negativeModifier = defendingWeaponData.effectiveAgainst.HasFlag(equippedWeaponData.weaponType) ? 0.5f : 0f;
        float modifier = positiveModifier - negativeModifier;

        return Mathf.RoundToInt(equippedWeaponData.power * (1 + stats.strength * 0.05f) * modifier);
    }

    // Checks if the attack crits.
    private bool GetCriticalHit() => Random.value < critChance;
    // Checks if the attack hits.
    private bool GetAttackHit() => Random.value < accuracy;
    #endregion

    #region Stats & Leveling
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

    // Updates the derived stat values based on the base stats
    private void UpdateStatValues()
    {
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        critChance = Mathf.Clamp01(stats.luck * 0.005f);
        accuracy = Mathf.Clamp01((stats.luck * 0.01f) + (stats.strength * 0.01f) + (stats.mastery * 0.02f));

        if (equippedWeapon.definition != null)
        {
            BaseWeapon equippedWeaponData = equippedWeapon.definition as BaseWeapon;
            attackRange = Mathf.Clamp(attackRange, equippedWeaponData.minRange, equippedWeaponData.maxRange);
            critChance = Mathf.Clamp01(equippedWeaponData.critChance + (stats.luck * 0.005f));
            accuracy = Mathf.Clamp01(accuracy * equippedWeaponData.weight);
        }

        maxHealth = 1 + Mathf.RoundToInt(stats.endurance * 0.25f);
        physicalDefense = Mathf.RoundToInt((stats.endurance * 0.02f) + (stats.strength * 0.01f));
        critModifier = 1.1f + (stats.mastery * 0.01f);
    }

    // Levels up the unit
    private void LevelUpOnce()
    {
        if (isAI) return;

        experience -= experienceToLevel;
        experienceToLevel *= 1.5f;
        level++;

        stats.LevelUpStats(unitClass.classGrowths, characterGrowths);
        UpdateStatValues();

        Debug.Log($"{unitName} leveled up to {level}!");
    }
    #endregion

    #region UI & PointerEvents
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

        if (distance <= playerUnit.attackRange)
        {
            if (playerUnit.attacksLeft > 0)
            {
                playerUnit.Attack(this);
            }
            else
            {
                return;
            }
        }

        Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, playerUnit);

        if (closestAttackTile == null)
        {
            return;
        }

        List<Tile> path = owner.gridManager.GetPath(attackerTile, closestAttackTile);

        if (path == null || path.Count == 0)
        {
            return;
        }

        playerUnit.MoveTo(path);
    }

    // Called when the pointer starts hovering over the unit.
    public void OnPointerEnter(PointerEventData eventData) => Player.hoverUnit = this;
    // Called when the pointer stops hovering over the unit.
    public void OnPointerExit(PointerEventData eventData) => Player.hoverUnit = null;
    #endregion

    #region Helpers
    // Check if the unit has been used up for the turn
    public bool IsUnitExpended() => movementLeft <= 0 && attacksLeft <= 0;

    // Changes the equipped weapon and updates stats accordingly
    public void ChangeEquippedWeapon(ItemInstance weapon)
    {
        equippedWeapon = weapon;
        UpdateStatValues();
        owner.ChangeSelectedUnit(this);
    }

    // Expend the unit for the turn
    public void ExpendUnit()
    {
        movementLeft = 0;
        attacksLeft = 0;
    }

    // Handles transferring loot to the killing unit or the convoy
    private void TransferLoot(Unit killer)
    {
        ConvoyManager convoyManager = FindAnyObjectByType<ConvoyManager>();
        UIManager uIManager = FindAnyObjectByType<UIManager>();

        foreach(ItemInstance item in inventory.items.ToList())
        {
            if (!item.droppable) continue;

            if(killer != null && killer.inventory.AddItem(item))
            {
                uIManager.PopulateInventory();
                inventory.RemoveItem(item);
                continue;
            }

            if(convoyManager!= null && convoyManager.inventory.AddItem(item))
            {
                uIManager.RefreshConvoy();
                inventory.RemoveItem(item);
            }
        }
    }

    // Handles actions to be performed on unit death
    private void Die(Unit killer)
    {
        TransferLoot(killer);
        owner.playerUnits.Remove(this);
        Destroy(gameObject);
    }
    #endregion
}