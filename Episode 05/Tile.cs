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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        tileRenderer = GetComponentInChildren<Renderer>();
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
            ChangeColor(inMoveRange? Color.cyan : originalColor);
        }
    }

    // Called when the mouse is clicked, selects this tile and highlights the movement range as if there were a unit positioned on it
    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectedTile)
        {
            selectedTile.ChangeColor(selectedTile.originalColor);
        }

        selectedTile = this;
        ChangeColor(selectedColor);

        FindAnyObjectByType<GridManager>().HighlightMoveRange(this, FindAnyObjectByType<Unit>().movementRange);
    }
}
