using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 10;
    public int height = 10;
    public GameObject tilePrefab;
    public string valueMap;
    public const int MAX_MOVE_COST = 5;
    private Tile[,] map;
    private float halfWidth => width / 2f;
    private float halfHeight => height / 2f;

    [Header("Materials")]
    public Material tileMaterial;
    public Gradient terrainColors;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        map = new Tile[width, height];
        GenerateGrid();
    }

    // Instantiates a grid of tiles with colors based on movement cost
    public void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Instantiate
                Vector3 tilePosition = new Vector3(x - halfWidth, 0, y - halfHeight);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tile.name = $"Tile {x},{y}";
                tile.transform.SetParent(transform);

                Tile tileScript = tile.GetComponent<Tile>();

                // Assign Color
                tileScript.tileRenderer.material = new Material(tileMaterial);
                int index = y * width + x;
                tileScript.moveCost = valueMap[index] - '0';
                float normalizedCost = (float)tileScript.moveCost / MAX_MOVE_COST;
                tileScript.originalColor = tileScript.moveCost > MAX_MOVE_COST? Color.red : terrainColors.Evaluate(normalizedCost);

                // Assign Properties
                tileScript.gridPosition = new Vector2Int(x, y);
                tileScript.gridManager = this;
                map[x, y] = tileScript;
            }
        }
    }

    // Retrieves the neighbors of the given tile position
    private List<Tile> GetTileNeighbors(Vector2Int tilePosition, bool includeDiagonals)
    {
        List<Tile> neighbors = new List<Tile>(8);
        Tile centerTile = map[tilePosition.x, tilePosition.y];

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int posX = tilePosition.x + x;
                int posY = tilePosition.y + y;

                if (posX < 0 || posY < 0 || posX >= width || posY >= height) continue;

                if (!includeDiagonals && x != 0 && y != 0) continue;

                neighbors.Add(map[posX, posY]);
            }
        }

        return neighbors;
    }

    // Checks it tile a is diagonal to tile b
    private bool IsDiagonal(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return dx == 1 && dy == 1;
    }

    // Removes all movement range highlighting on the map
    public void ResetGridHighlights()
    {
        foreach (Tile tile in map)
        {
            if (Tile.selectedTile != tile)
            {
                tile.inMoveRange = false;
                tile.inAttackRange = false;
                tile.ChangeColor(tile.originalColor);
            }
        }
    }

    // Uses Dijkstra's algorithm to find all tiles reachable within range of a given position
    public List<Tile> GetHighlightRange(Vector2Int start, int moveRange, int attackRange)
    {
        ResetGridHighlights();

        HashSet<Tile> moveTiles = new HashSet<Tile>();
        Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();
        Queue<Tile> edge = new Queue<Tile>();

        Tile startTile = map[start.x, start.y];
        edge.Enqueue(startTile);
        costSoFar[startTile] = 0;

        while (edge.Count > 0)
        {
            Tile current = edge.Dequeue();
            int currentCost = costSoFar[current];

            foreach (Tile neighbor in GetTileNeighbors(current.gridPosition, true))
            {
                int stepCost = IsDiagonal(current, neighbor) ? 1 + neighbor.moveCost : neighbor.moveCost;
                int newCost = currentCost + stepCost;

                if (newCost <= moveRange && (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) && !neighbor.isOccupied)
                {
                    if (!costSoFar.TryGetValue(neighbor, out int prevCost) || newCost < prevCost)
                    {
                        costSoFar[neighbor] = newCost;
                        edge.Enqueue(neighbor);

                        if (neighbor != startTile && moveTiles.Add(neighbor))
                        {
                            neighbor.inMoveRange = true;
                        }
                    }
                }
            }
        }

        HashSet<Tile> attackTiles = new HashSet<Tile>();
        Queue<(Tile tile, int distance)> attackQueue = new Queue<(Tile tile, int distance)>();

        foreach(Tile origin in moveTiles)
        {
            attackQueue.Enqueue((origin, 0));
            attackQueue.Enqueue((startTile, 0));

            HashSet<Tile> visited = new HashSet<Tile> { startTile };

            while(attackQueue.Count > 0)
            {
                var (tile, distance) = attackQueue.Dequeue();

                foreach(Tile neighbor in GetTileNeighbors(tile.gridPosition, false))
                {
                    if(!visited.Contains(neighbor) && distance + 1 <= attackRange)
                    {
                        visited.Add(neighbor);
                        attackQueue.Enqueue((neighbor, distance + 1));
                        attackTiles.Add(neighbor);
                        neighbor.inAttackRange = true;
                    }
                }
            }
        }

        List<Tile> result = new List<Tile>(moveTiles.Count + attackTiles.Count);
        result.AddRange(attackTiles);
        result.AddRange(moveTiles);
        return result;
    }

    // Highlights all tiles within range of the start tile
    public void HighlightRange(Tile start, int moveRange, int attackRange)
    {
        List<Tile> reachableTiles = GetHighlightRange(start.gridPosition, moveRange, attackRange);

        foreach (Tile tile in reachableTiles)
        {
            if (tile.inMoveRange)
            {
                tile.ChangeColor(Color.cyan);
            }
            else if (tile.inAttackRange)
            {
                tile.ChangeColor(Color.red);
            }
        }
    }

    // Retrieves the tile at the given grid position
    public Tile GetTile(Vector2Int position)
    {
        return map[position.x, position.y];
    }

    // Retrieves the tile at the given world position
    public Tile GetTile(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x + halfWidth);
        int y = Mathf.RoundToInt(position.z + halfHeight);

        return map[x, y];
    }

    // Heuristic function for A* pathfinding (Manhattan distance)
    public int GetHeuristic (Tile start, Tile end)
    {
        return Mathf.Abs(start.gridPosition.x - end.gridPosition.x) + Mathf.Abs(start.gridPosition.y - end.gridPosition.y);
    }

    // Retraces the path from end to start using parent references
    private List<Tile> RetracePath(Tile start, Tile end)
    {
        List<Tile> path = new List<Tile>();

        Tile current = end;

        while(current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    // Implements the A* pathfinding algorithm to find the shortest path from start to end
    public List<Tile> GetPath(Tile start, Tile end, Tile ignoreOccupied = null)
    {
        var open = new PriorityQueue<Tile>();
        HashSet<Tile> closed = new HashSet<Tile>();

        foreach(Tile t in map)
        {
            t.gCost = int.MaxValue;
            t.hCost = 0;
            t.parent = null;
        }

        start.gCost = 0;
        start.hCost = GetHeuristic(start, end);
        start.parent = null;

        open.Enqueue(start);

        while(open.Count > 0)
        {
            Tile current = open.Dequeue();
            if(current == end) return RetracePath(start, end);

            closed.Add(current);

            foreach(Tile neighbor in GetTileNeighbors(current.gridPosition, true))
            {
                if(closed.Contains(neighbor)) continue;
                if (neighbor.isOccupied && neighbor != ignoreOccupied) continue;

                int tempG = current.gCost + neighbor.moveCost;

                if(tempG < neighbor.gCost || neighbor.parent == null)
                {
                    neighbor.gCost = tempG;
                    neighbor.hCost = GetHeuristic(neighbor, end);
                    neighbor.parent = current;

                    open.Enqueue(neighbor);
                }
            }
        }

        return null;
    }

    // Returns a tile ideal for attacking from
    public Tile GetClosestAttackTile(Unit target, Unit attacker)
    {
        Tile targetTile = GetTile(target.gridPosition);
        Tile closestTile = null;
        int bestDistance = int.MaxValue;
        int attackRange = attacker.attackRange;

        foreach(Tile tile in map)
        {
            if (!tile.inMoveRange || tile.isOccupied) continue;

            int distanceToTarget = GetHeuristic(targetTile, tile);
            if (distanceToTarget > attackRange) continue;

            int distanceToAttacker = GetHeuristic(GetTile(attacker.gridPosition), tile);

            if(distanceToAttacker < bestDistance)
            {
                closestTile = tile;
                bestDistance = distanceToAttacker;
            }
        }

        return closestTile;
    }
}