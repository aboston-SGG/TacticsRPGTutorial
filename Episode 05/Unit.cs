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

    public void MoveTo(Vector3 position, Vector2Int gridPos)
    {
        targetPosition = position;
        gridPosition = gridPos;
        isMoving = true;
    }

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

    public void OnPointerDown(PointerEventData eventData)
    {
        owner.ChangeSelectedUnit(this);
    }
}
