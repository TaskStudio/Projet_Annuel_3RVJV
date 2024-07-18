using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private readonly float cellSize;
    private readonly Dictionary<Vector2Int, List<Unit>> grid;

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
        if (!grid.ContainsKey(cell)) grid[cell] = new List<Unit>();
        grid[cell].Add(unit);
    }

    public void Remove(Unit unit)
    {
        Vector2Int cell = GetCell(unit.transform.position);
        if (grid.ContainsKey(cell))
        {
            grid[cell].Remove(unit);
            if (grid[cell].Count == 0) grid.Remove(cell);
        }
    }

    public void Update(Unit entity, Vector3 oldPosition)
    {
        Vector2Int oldCell = GetCell(oldPosition);
        Vector2Int newCell = GetCell(entity.transform.position);
        if (oldCell != newCell)
        {
            if (grid.ContainsKey(oldCell))
            {
                grid[oldCell].Remove(entity);
                if (grid[oldCell].Count == 0) grid.Remove(oldCell);
            }

            Add(entity);
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