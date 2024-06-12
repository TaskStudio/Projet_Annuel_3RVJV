using UnityEngine;

internal interface IMap
{
    Zone AllianceZone { get; }
    Zone MonsterZone { get; }
    Vector3 ClampPositionToLimits(Vector3 position);
}