using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGridData : IGridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects { get; } = new();

    public bool IsPositionOccupied(Vector3Int gridPosition)
    {
        return placedObjects.ContainsKey(gridPosition);
    }

    public List<Vector3Int> AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID)
    {
        List<Vector3Int> positionsToOccupy = CalculatePosition(gridPosition, objectSize);
        var data = new PlacementData(positionsToOccupy, ID);
        foreach (Vector3Int pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Position {pos} is already occupied");
            placedObjects[pos] = data;
        }

        return positionsToOccupy;
    }

    public void RemoveObjectAt(List<Vector3Int> gridPositions)
    {
        foreach (Vector3Int pos in gridPositions)
            placedObjects.Remove(pos);
    }

    private List<Vector3Int> CalculatePosition(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positions = new();
        for (var x = 0; x < objectSize.x; x++)
        for (var y = 0; y < objectSize.y; y++)
            positions.Add(gridPosition + new Vector3Int(x, 0, y));
        return positions;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePosition(gridPosition, objectSize);
        foreach (Vector3Int pos in positionsToOccupy)
            if (placedObjects.ContainsKey(pos))
                return false;
        return true;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public PlacementData(List<Vector3Int> occupiedPositions, int ID)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
    }

    public int ID { get; }
}