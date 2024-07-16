using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public abstract class Unit : Unit<UnitData>
{
}

public class Unit<TDataType> : Entity<TDataType> where TDataType : UnitData
{
    private static SpatialGrid spatialGrid;
    [SerializeField] protected float avoidanceStrength = 5f;
    [SerializeField] protected float collisionRadius = 1f;
    [Space(10)] [Header("Movement")]
    [SerializeField] protected LayerMask entityLayer;

    private Vector3 avoidanceVector;
    private Collider entityCollider;
    private bool isMoving;
    private Vector3 lastPosition;
    private bool needsCollisionAvoidance;
    private Vector3 originalTargetPosition;
    protected float stoppingDistance = 0.01f;
    protected Vector3 targetPosition;
    public int currentMana { get; protected set; }
    public float movementSpeed { get; protected set; } = 0.5f;
    public float attackSpeed { get; protected set; } = 1.0f;

    protected void Start()
    {
        if (spatialGrid == null) spatialGrid = new SpatialGrid(5f);

        spatialGrid.Add(this as Unit);
        lastPosition = transform.position;

        UnitsManager.Instance.RegisterMovableEntity(this as Unit);
        targetPosition = transform.position;
        originalTargetPosition = transform.position;
        entityCollider = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
            return; // Early exit if the unit is destroyed or inactive

        spatialGrid.Update(this as Unit, lastPosition);
        lastPosition = transform.position;

        float distanceToTarget = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(targetPosition.x, targetPosition.z)
        );

        if (distanceToTarget > stoppingDistance || !isMoving)
        {
            if (!isMoving) isMoving = true;
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
        }
        else if (distanceToTarget <= stoppingDistance && isMoving)
        {
            isMoving = false;
            targetPosition = transform.position;
            needsCollisionAvoidance = true;
        }

        if (needsCollisionAvoidance && Vector3.Distance(transform.position, originalTargetPosition) > stoppingDistance)
        {
            Move(originalTargetPosition);
            needsCollisionAvoidance = false;
        }

        avoidanceVector = Vector3.zero;
    }

    protected override void Initialize()
    {
        base.Initialize();
        currentMana = data.maxManaPoints;
        attackSpeed = data.attackSpeed;
        movementSpeed = data.movementSpeed;
    }

    public void Move(Vector3 newPosition)
    {
        if (this != null)
        {
            targetPosition = new Vector3(
                newPosition.x,
                transform.position.y,
                newPosition.z
            );
            originalTargetPosition = targetPosition;
            isMoving = true;
            needsCollisionAvoidance = false;
        }
    }

    public void MoveInFormation(Vector3 targetPosition)
    {
        List<Unit> selectedEntities = new();

        foreach (var Unit in UnitsManager.MovableUnits)
            if (Unit is Unit selectable && selectable.IsSelected)
                selectedEntities.Add(Unit);

        int numSelected = selectedEntities.Count;
        if (numSelected == 0) return;

        Unit firstEntity = selectedEntities[0];

        float collisionRadius = firstEntity.collisionRadius;
        float offset = 0.1f;
        float spacing = collisionRadius * 1.8f + offset;

        int entitiesPerRow = Mathf.CeilToInt(Mathf.Sqrt(numSelected));
        float totalWidth = entitiesPerRow * spacing;
        float totalHeight = Mathf.CeilToInt((float) numSelected / entitiesPerRow) * spacing;

        Vector3 topLeftPosition = targetPosition - new Vector3(totalWidth / 2, 0, totalHeight / 2);

        for (int i = 0; i < selectedEntities.Count; i++)
        {
            int row = i / entitiesPerRow;
            int column = i % entitiesPerRow;

            Vector3 offsetPosition = new Vector3(column * spacing, 0, row * spacing);
            selectedEntities[i].Move(topLeftPosition + offsetPosition);
        }
    }


    private Vector3 AvoidCollisions()
    {
        List<Unit> neighbors = spatialGrid.GetNeighbors(transform.position);
        NativeArray<Vector3> unitPositions = new(neighbors.Count, Allocator.TempJob);
        for (int i = 0; i < neighbors.Count; i++)
            if (neighbors[i] != null && neighbors[i].gameObject.activeInHierarchy)
                unitPositions[i] = neighbors[i].transform.position;

        NativeArray<Vector3> avoidanceVectorArray = new(1, Allocator.TempJob);

        var job = new AvoidCollisionsJob
        {
            unitPositions = unitPositions,
            currentPosition = transform.position,
            collisionRadius = collisionRadius,
            avoidanceStrength = avoidanceStrength,
            avoidanceVector = avoidanceVectorArray
        };

        JobHandle handle = job.Schedule();
        handle.Complete();

        Vector3 avoidance = avoidanceVectorArray[0];

        unitPositions.Dispose();
        avoidanceVectorArray.Dispose();

        return targetPosition + avoidance;
    }

    private void MoveTowardsTarget(Vector3 adjustedPosition)
    {
        float distanceToTarget = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(adjustedPosition.x, adjustedPosition.z)
        );
        if (distanceToTarget <= stoppingDistance) return;

        NativeArray<Vector3> newPositionArray = new(1, Allocator.TempJob);

        var moveJob = new MoveJob
        {
            currentPosition = transform.position,
            targetPosition = adjustedPosition,
            moveSpeed = movementSpeed,
            deltaTime = Time.deltaTime,
            newPosition = newPositionArray
        };

        JobHandle moveJobHandle = moveJob.Schedule();
        moveJobHandle.Complete();

        Vector3 newPosition = newPositionArray[0];
        newPositionArray.Dispose();

        transform.position = newPosition;
    }


    protected override void Die()
    {
        UnitFactory.ReturnEntity(this as Unit);
    }
}