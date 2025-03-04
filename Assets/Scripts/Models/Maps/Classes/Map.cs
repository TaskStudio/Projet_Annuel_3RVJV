using UnityEngine;

public class Map : MonoBehaviour, IMap
{
    private bool _isMapColliderNull;

    private Collider _mapCollider; // Reference to the map's collider

    private void Awake()
    {
        // Attempt to get the collider component attached to the GameObject
        _mapCollider = GetComponent<Collider>();
        if (_mapCollider == null)
            Debug.LogError(
                "Map Collider is not found. Please attach a Collider component to define the map boundaries."
            );
    }

    private void Start()
    {
        _isMapColliderNull = _mapCollider == null;
    }

    public Zone AllianceZone { get; }
    public Zone MonsterZone { get; }

    // Clamps a position within the map's limits, using the collider's bounds
    public Vector3 ClampPositionToLimits(Vector3 position)
    {
        if (_isMapColliderNull) return position;

        var clampedPosition = position;

        // Clamp each coordinate within the collider's bounds
        var bounds = _mapCollider.bounds;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, bounds.min.x, bounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, bounds.min.y, bounds.max.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, bounds.min.z, bounds.max.z);

        return clampedPosition;
    }

    // Checks if a position is within the map's boundaries, using the collider's bounds
    public bool IsWithinLimits(Vector3 position)
    {
        if (_mapCollider == null) return false;

        return _mapCollider.bounds.Contains(position);
    }
}