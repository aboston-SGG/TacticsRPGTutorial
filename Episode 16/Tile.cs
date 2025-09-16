using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IComparable<Tile>
{
    public Vector2Int gridPosition;

    [Header("Colors")]
    public Color originalColor;
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.blue;

    public Renderer tileRenderer;
    public static Tile selectedTile;
    public bool inMoveRange = false;
    public bool inAttackRange = false;

    public int moveCost = 0;

    public bool isOccupied = false;

    public int gCost = int.MaxValue;
    public int hCost = 0;
    public int fCost => gCost + hCost;

    public Tile parent;

    public GridManager gridManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        tileRenderer = GetComponentInChildren<Renderer>();
        ChangeColor(originalColor);
    }

    // Changes the color of this tile
    public void ChangeColor(Color newColor)
    {
        tileRenderer.material.color = newColor;
    }

    // Called when the pointer first hovers over an object, adds the highlight to this tile
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedTile != this)
        {
            ChangeColor(highlightColor);
        }
    }

    // Called when the pointer stops hovering over an object, changes tile color based on if it's in range or not
    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedTile == this) return;

        if (inMoveRange)
        {
            ChangeColor(Color.cyan);
        }
        else if (inAttackRange)
        {
            ChangeColor(Color.red);
        }
        else
        {
            ChangeColor(originalColor);
        }
    }

    // Called when the mouse is clicked, selects this tile and moves the selected unit to this if the tile is in range, deselects if it isn't
    public void OnPointerDown(PointerEventData eventData)
    {
        Unit selected = Player.selectedUnit;

        if (selected == null || selected.isMoving) return;
        if (!inMoveRange) return;

        Tile startTile = gridManager.GetTile(selected.gridPosition);

        List<Tile> path = gridManager.GetPath(startTile, this, startTile);

        if (path == null || path.Count == 0) return;

        int totalCost = 0;

        foreach (Tile t in path) totalCost += t.moveCost;

        if (totalCost > selected.movementLeft) return;

        selected.MoveTo(path);

        SelectTile();

        gridManager.ResetGridHighlights();
    }

    // Selects this tile and changes its color
    public void SelectTile()
    {
        if (selectedTile)
        {
            selectedTile.ChangeColor(selectedTile.originalColor);
        }

        selectedTile = this;
        ChangeColor(selectedColor);
    }

    // Compares based on fCost, then by hCost
    public int CompareTo(Tile other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0) compare = hCost.CompareTo(other.hCost);

        return compare;
    }
}