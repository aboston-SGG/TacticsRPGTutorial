using UnityEngine;

public class Player : MonoBehaviour
{
    public static Unit selectedUnit;
    public GridManager gridManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
    }

    // Changes the selected unit and highlights the movement range of that unit
    public void ChangeSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        gridManager.HighlightMoveRange(gridManager.GetTile(unit.gridPosition), unit.movementRange);
    }
}
