using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public string playerName;

    public static Unit selectedUnit;
    public static Unit hoverUnit;

    public GridManager gridManager;

    public List<Unit> playerUnits;

    public bool isPlayerTurn = false;
    public bool readyToEndTurn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(gridManager == null) gridManager = FindAnyObjectByType<GridManager>();
        SnapUnits();
    }

    // Update is called once per frame
    private void Update()
    {
        if(isPlayerTurn)
        {
            CheckUnits();
        }
    }

    // Changes the selected unit and highlights the movement range of that unit
    public void ChangeSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        Tile unitTile = gridManager.GetTile(unit.gridPosition);
        unitTile.SelectTile();
        gridManager.HighlightRange(unitTile, unit.movementLeft, unit.attackRange);
    }

    // Snaps all player units to the nearest tile and updates their grid position
    private void SnapUnits()
    {
        foreach(Unit unit in playerUnits)
        {
            Tile unitTile = gridManager.GetTile(unit.transform.position);
            unit.gridPosition = unitTile.gridPosition;
            unit.transform.position = unitTile.transform.position;
            unitTile.isOccupied = true;
            unit.owner = this;
        }
    }

    // Reset the movement left for each unit
    public void ResetUnits()
    {
        foreach(Unit unit in playerUnits)
        {
            unit.movementLeft = unit.movementRange;
            unit.attacksLeft = 1;
        }
    }

    // Checks if all units have expended their actions
    private void CheckUnits()
    {
        foreach(Unit unit in playerUnits)
        {
            if (!unit.IsUnitExpended())
            {
                return;
            }
        }

        readyToEndTurn = true;
    }
}