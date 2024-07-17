using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public interface IUnit : IEntity
{
    float CollisionRadius { get; }

    void Move(Vector3 newPosition);
    void MoveInFormation(Vector3 targetPosition);
    void SetTarget(IBaseObject target);
}


public abstract class Unit : Unit<UnitData>
{
}

[Serializable]
public abstract class Unit<TDataType> : Entity<TDataType>, IUnit where TDataType : UnitData
{
    protected static SpatialGrid spatialGrid;

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
    protected bool reachedDestination;
    protected float stoppingDistance = 0.1f;
    protected Vector3 targetPosition;
    private IUnit unitImplementation;
    public int currentMana { get; protected set; }
    public float movementSpeed { get; protected set; } = 0.5f;
    public float attackSpeed { get; protected set; } = 1.0f;

    protected void Start()
    {
        if (spatialGrid == null) spatialGrid = new SpatialGrid(5f);

        spatialGrid.Add(this);
        lastPosition = transform.position;

        if (this is IAlly)
            UnitsManager.Instance.RegisterMovableEntity(this);
        targetPosition = transform.position;
        originalTargetPosition = transform.position;
        entityCollider = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
            return; // Early exit if the unit is destroyed or inactive

        spatialGrid.Update(this, lastPosition);
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
            Stop();
            needsCollisionAvoidance = true;
        }

        if (needsCollisionAvoidance
            && !reachedDestination
            && Vector3.Distance(transform.position, originalTargetPosition) > stoppingDistance)
        {
            Move(originalTargetPosition);
            needsCollisionAvoidance = false;
        }

        avoidanceVector = Vector3.zero;
    }

    public float CollisionRadius => collisionRadius;

    public virtual void Move(Vector3 newPosition)
    {
        targetPosition = new Vector3(
            newPosition.x,
            transform.position.y,
            newPosition.z
        );
        originalTargetPosition = targetPosition;
        isMoving = true;
        needsCollisionAvoidance = false;
        reachedDestination = false;
    }

    public virtual void MoveInFormation(Vector3 targetFormationPosition)
    {
        List<IUnit> selectedEntities = new();

        foreach (var unit in UnitsManager.MovableUnits)
            if (unit.IsSelected)
                selectedEntities.Add(unit);

        int numSelected = selectedEntities.Count;
        if (numSelected == 0) return;

        IUnit firstEntity = selectedEntities[0];

        float firstEntityCollisionRadius = firstEntity.CollisionRadius;
        float offset = 0.1f;
        float spacing = firstEntityCollisionRadius * 1.8f + offset;

        int entitiesPerRow = Mathf.CeilToInt(Mathf.Sqrt(numSelected));
        float totalWidth = entitiesPerRow * spacing;
        float totalHeight = Mathf.CeilToInt((float) numSelected / entitiesPerRow) * spacing;

        Vector3 topLeftPosition = targetFormationPosition - new Vector3(totalWidth / 2, 0, totalHeight / 2);

        for (int i = 0; i < selectedEntities.Count; i++)
        {
            int row = i / entitiesPerRow;
            int column = i % entitiesPerRow;

            Vector3 offsetPosition = new Vector3(column * spacing, 0, row * spacing);
            selectedEntities[i].Move(topLeftPosition + offsetPosition);
        }
    }

    public override void SignalDeath()
    {
        base.SignalDeath();
        UnitsManager.Instance.UnregisterMovableEntity(this);
    }

    public abstract void SetTarget(IBaseObject target);

    public void Stop()
    {
        reachedDestination = true;
        targetPosition = transform.position;
        isMoving = false;
    }

    protected override void Initialize()
    {
        base.Initialize();
        currentMana = data.maxManaPoints;
        attackSpeed = data.attackSpeed;
        movementSpeed = data.movementSpeed;
    }


    private Vector3 AvoidCollisions()
    {
        List<IEntity> neighbors = spatialGrid.GetNeighbors(transform.position);
        NativeArray<Vector3> unitPositions = new(neighbors.Count, Allocator.TempJob);
        foreach (var neighbor in neighbors)
            if (neighbor != null && neighbor.gameObject.activeInHierarchy)
                unitPositions[neighbors.IndexOf(neighbor)] = neighbor.transform.position;

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
        UnitFactory.ReturnEntity(this);
    }
}