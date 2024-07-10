using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private Dictionary<Vector2Int, List<Unit>> grid;
    private float cellSize;

    public SpatialGrid(float cellSize)
    {
        this.cellSize = cellSize;
        grid = new Dictionary<Vector2Int, List<Unit>>();
    }

    public Vector2Int GetCell(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    public void Add(Unit unit)
    {
        Vector2Int cell = GetCell(unit.transform.position);
        if (!grid.ContainsKey(cell))
        {
            grid[cell] = new List<Unit>();
        }
        grid[cell].Add(unit);
    }

    public void Update(Unit unit, Vector3 oldPosition)
    {
        Vector2Int oldCell = GetCell(oldPosition);
        Vector2Int newCell = GetCell(unit.transform.position);
        if (oldCell != newCell)
        {
            grid[oldCell].Remove(unit);
            Add(unit);
        }
    }

    public List<Unit> GetNeighbors(Vector3 position)
    {
        Vector2Int cell = GetCell(position);
        List<Unit> neighbors = new List<Unit>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2Int neighborCell = new Vector2Int(cell.x + x, cell.y + z);
                if (grid.ContainsKey(neighborCell))
                {
                    neighbors.AddRange(grid[neighborCell]);
                }
            }
        }
        return neighbors;
    }
}