using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Unit : MonoBehaviour, IPointerDownHandler
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

    void Start()
    {
        stats = new UnitStats(Random.Range(0, 100), Random.Range(0, 100));
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        attackRange = Mathf.Clamp(attackRange, 1, int.MaxValue);
        movementLeft = movementRange;
    }

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

            movementLeft -= path[0].moveCost;
            path.RemoveAt(0);

            if(path.Count > 0)
            {
                MoveTo(path[0].transform.position, path[0].gridPosition);
            }
            else
            {
                owner.ChangeSelectedUnit(this);
            }
        }
    }

    // Called when the mouse is clicked, changes the selected unit to this unit
    public void OnPointerDown(PointerEventData eventData)
    {
        owner.ChangeSelectedUnit(this);
    }
}