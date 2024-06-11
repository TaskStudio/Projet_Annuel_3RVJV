using System.Collections.Generic;
using Construction;
using UnityEngine;

namespace Maps.Interfaces
{
    public interface IGridData
    {
        Dictionary<Vector3Int, PlacementData> placedObjects { get; }
        bool IsPositionOccupied(Vector3Int gridPosition);
    }
}