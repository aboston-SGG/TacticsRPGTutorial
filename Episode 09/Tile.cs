using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Vector2Int gridPosition;

    [Header("Colors")]
    public Color originalColor;
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.blue;

    private Renderer tileRenderer;
    public static Tile selectedTile;
    public bool inMoveRange = false;

    public int moveCost = 0;

    public bool isOccupied = false;

    public int gCost = int.MaxValue;
    public int hCost = 0;
    public int fCost => gCost + hCost;

    public Tile parent;

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
        if (selectedTile != this)
        {
            ChangeColor(inMoveRange ? Color.cyan : originalColor);
        }
    }

    // Called when the mouse is clicked, selects this tile and moves the selected unit to this if the tile is in range, deselects if it isn't
    public void OnPointerDown(PointerEventData eventData)
    {
        SelectTile();

        GridManager gridManager = GetComponentInParent<GridManager>();

        if (Player.selectedUnit)
        {
            if (inMoveRange)
            {
                gridManager.GetTile(Player.selectedUnit.gridPosition).isOccupied = false;
                Player.selectedUnit.path = gridManager.GetPath(gridManager.GetTile(Player.selectedUnit.gridPosition), this);
                Player.selectedUnit.MoveTo(Player.selectedUnit.path[0].transform.position, Player.selectedUnit.path[0].gridPosition);
                isOccupied = true;
            }
            else
            {
                Player.selectedUnit = null;
            }
        }
    }

    // Selects this tile and changes its color
    private void SelectTile()
    {
        if (selectedTile)
        {
            selectedTile.ChangeColor(selectedTile.originalColor);
        }

        selectedTile = this;
        ChangeColor(selectedColor);
    }
}