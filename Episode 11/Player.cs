using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Unit selectedUnit;
    public GridManager gridManager;

    public List<Unit> playerUnits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        SnapUnits();
    }

    // Changes the selected unit and highlights the movement range of that unit
    public void ChangeSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        gridManager.HighlightRange(gridManager.GetTile(unit.gridPosition), unit.movementLeft, unit.attackRange);
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
        }
    }
}