using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private readonly float cellSize;
    private readonly Dictionary<Vector2Int, List<IUnit>> grid;

    public SpatialGrid(float cellSize)
    {
        this.cellSize = cellSize;
        grid = new Dictionary<Vector2Int, List<IUnit>>();
    }

    public Vector2Int GetCell(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    public void Add(IUnit unit)
    {
        Vector2Int cell = GetCell(unit.transform.position);
        if (!grid.ContainsKey(cell)) grid[cell] = new List<IUnit>();
        grid[cell].Add(unit);
    }

    public void Update(IUnit unit, Vector3 oldPosition)
    {
        Vector2Int oldCell = GetCell(oldPosition);
        Vector2Int newCell = GetCell(unit.transform.position);
        if (oldCell != newCell)
        {
            if (grid.ContainsKey(oldCell))
            {
                grid[oldCell].Remove(unit);
                if (grid[oldCell].Count == 0) grid.Remove(oldCell);
            }

            Add(unit);
        }
    }

    public List<IUnit> GetNeighbors(Vector3 position)
    {
        Vector2Int cell = GetCell(position);
        List<IUnit> neighbors = new();

        for (int x = -1; x <= 1; x++)
        for (int z = -1; z <= 1; z++)
        {
            Vector2Int neighborCell = new Vector2Int(cell.x + x, cell.y + z);
            if (grid.ContainsKey(neighborCell)) neighbors.AddRange(grid[neighborCell]);
        }

        return neighbors;
    }
}