using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    public Vector2Int gridPosition;
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private const float STOPPING_DISTANCE = 0.01f;

    public int movementRange = 3;

    public Player owner;

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    // Initialize the movement values, called when we want to move our unit
    public void MoveTo(Vector3 position, Vector2Int gridPos)
    {
        targetPosition = position;
        gridPosition = gridPos;
        isMoving = true;
    }

    // Handles the actual movement of the unit, called every frame
    private void HandleMovement()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < STOPPING_DISTANCE)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    // Called when the mouse is clicked, changes the selected unit to this unit
    public void OnPointerDown(PointerEventData eventData)
    {
        owner.ChangeSelectedUnit(this);
    }
}
