using System.Collections.Generic;
using System.Linq;
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

    public List<Unit> GetNeighbors(Vector3 position, int range = 1)
    {
        Vector2Int cell = GetCell(position);
        List<Unit> neighbors = new();

        for (int x = -range; x <= range; x++)
        for (int z = -range; z <= range; z++)
        {
            Vector2Int neighborCell = new Vector2Int(cell.x + x, cell.y + z);
            if (grid.ContainsKey(neighborCell))
            {
                var neighborsToAdd = grid[neighborCell].Where(unit => !unit.IsDead());
                neighbors.AddRange(neighborsToAdd);
            }
        }

        return neighbors;
    }

    public List<Unit> GetNeighborsByFaction<T>(Vector3 position, int range = 1) where T : IFaction
    {
        Vector2Int cell = GetCell(position);
        List<Unit> neighbors = new();

        for (int x = -range; x <= range; x++)
        for (int z = -range; z <= range; z++)
        {
            Vector2Int neighborCell = new Vector2Int(cell.x + x, cell.y + z);
            if (!grid.TryGetValue(neighborCell, out List<Unit> entities)) continue;

            foreach (var entity in entities)
                if (!entity.IsDead() && entity is T)
                    neighbors.Add(entity);
        }

        return neighbors;
    }
}