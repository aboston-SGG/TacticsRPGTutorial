using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 10;
    public int height = 10;
    public GameObject tilePrefab;
    public string valueMap;
    public const int MAX_MOVE_COST = 5;

    [Header("Materials")]
    public Material lightMaterial;
    public Material darkMaterial;
    public Gradient terrainColors;

    private Tile[,] map;

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
                Vector3 tilePosition = new Vector3(x - width / 2, 0, y - height / 2);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tile.name = $"Tile {x},{y}";
                tile.transform.SetParent(transform);

                Renderer renderer = tile.GetComponentInChildren<Renderer>();

                renderer.material = new Material((x + y) % 2 == 0 ? lightMaterial : darkMaterial);

                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.gridPosition = new Vector2Int(x, y);
                tileScript.moveCost = valueMap[x * width + y] - '0';
                float normalizedCost = (float)tileScript.moveCost / MAX_MOVE_COST;
                tileScript.originalColor = tileScript.moveCost > MAX_MOVE_COST? Color.red : terrainColors.Evaluate(normalizedCost);
                map[x, y] = tileScript;
            }
        }
    }

    // Retrieves the neighbors of the given tile position
    private List<Tile> GetTileNeighbors(Vector2Int tilePosition)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int posX = tilePosition.x + x;
                int posY = tilePosition.y + y;

                if (posX >= 0 && posY >= 0 && posX < width && posY < height && new Vector2Int(posX, posY) != tilePosition)
                {
                    neighbors.Add(map[posX, posY]);
                }
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
    private void ResetGridHighlights()
    {
        foreach (Tile tile in map)
        {
            if (Tile.selectedTile != tile)
            {
                tile.inMoveRange = false;
                tile.ChangeColor(tile.originalColor);
            }
        }
    }

    // Uses Dijkstra's algorithm to find all tiles reachable within range of a given position
    public List<Tile> GetHighlightRange(Vector2Int start, int range)
    {
        ResetGridHighlights();

        List<Tile> reachable = new List<Tile>();
        Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();
        Queue<Tile> edge = new Queue<Tile>();

        Tile startTile = map[start.x, start.y];
        edge.Enqueue(startTile);
        costSoFar[startTile] = 0;

        while (edge.Count > 0)
        {
            Tile current = edge.Dequeue();
            int currentCost = costSoFar[current];

            foreach (Tile neighbor in GetTileNeighbors(current.gridPosition))
            {
                int stepCost = IsDiagonal(current, neighbor) ? 1 + neighbor.moveCost : neighbor.moveCost;
                int newCost = currentCost + stepCost;

                if (newCost <= range && (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) && !neighbor.isOccupied)
                {
                    costSoFar[neighbor] = newCost;
                    edge.Enqueue(neighbor);

                    if (!reachable.Contains(neighbor) && neighbor != startTile)
                    {
                        reachable.Add(neighbor);
                        neighbor.inMoveRange = true;
                    }
                }
            }
        }

        return reachable;
    }

    // Highlights all tiles within range of the start tile
    public void HighlightMoveRange(Tile start, int range)
    {
        List<Tile> reachableTiles = GetHighlightRange(start.gridPosition, range);

        foreach (Tile tile in reachableTiles)
        {
            tile.ChangeColor(Color.cyan);
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
        float offsetX = width / 2f;
        float offsetY = height / 2f;

        int x = Mathf.RoundToInt(position.x + offsetX);
        int y = Mathf.RoundToInt(position.z + offsetY);

        return map[x, y];
    }
}