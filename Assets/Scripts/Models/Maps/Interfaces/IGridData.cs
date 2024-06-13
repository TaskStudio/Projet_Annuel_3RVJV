using System.Collections.Generic;
using Construction;
using UnityEngine;

public interface IGridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects { get; }
    bool IsPositionOccupied(Vector3Int gridPosition);
}