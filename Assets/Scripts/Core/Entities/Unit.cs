using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Allocator = Unity.Collections.Allocator;

[Serializable]
public abstract class Unit : Entity
{
    protected static SpatialGrid spatialGrid;
    protected static Dictionary<Collider, Unit> colliderToUnitMap = new();

    [Space(10)] [Header("Movement")]
    [SerializeField] protected float avoidanceStrength = 5f;
    [SerializeField] protected float collisionRadius = 1f;

    [Space(10)] [Header("Mana")]
    [SerializeField] private ManaBar manaBar;

    private Vector3 avoidanceVector;
    private bool isMoving;
    private Vector3 lastPosition;
    private bool needsCollisionAvoidance;
    private Vector3 originalTargetPosition;
    protected bool reachedDestination;
    protected float stoppingDistance = 0.1f;
    protected Vector3 targetPosition;

    private Collider unitCollider;
    private Unit unitImplementation;

    public List<Unit> targetedBy { get; } = new();


    public int currentMana { get; protected set; }
    public float movementSpeed { get; protected set; } = 0.5f;
    public float attackSpeed { get; protected set; } = 1.0f;

    public float CollisionRadius => collisionRadius;

    protected void Start()
    {
        if (spatialGrid == null) spatialGrid = new SpatialGrid(5f);

        spatialGrid.Add(this);
        lastPosition = transform.position;

        if (this is Ally)
            UnitsManager.Instance.RegisterMovableEntity(this);
        targetPosition = transform.position;
        originalTargetPosition = transform.position;
        unitCollider = GetComponent<Collider>();
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

    private void OnEnable()
    {
        colliderToUnitMap[unitCollider] = this;
    }

    private void OnDisable()
    {
        colliderToUnitMap.Remove(unitCollider);
        SignalDeath();
    }

    public void AddTargetedBy(Unit unit)
    {
        if (!targetedBy.Contains(unit)) targetedBy.Add(unit);
    }

    public void RemoveTargetedBy(Unit unit)
    {
        if (targetedBy.Contains(unit)) targetedBy.Remove(unit);
    }

    public void SignalDeath()
    {
        foreach (Unit unit in targetedBy)
            if (unit != null)
            {
                unit.RemoveTargetedBy(this);
                unit.TargetIsDead(this);
            }

        SelectionManager.Instance.DeselectEntity(this);
        UnitsManager.Instance.UnregisterMovableEntity(this);
    }

    public abstract void TargetIsDead(Unit unit);

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
        List<Unit> selectedEntities = new();

        foreach (var unit in UnitsManager.MovableUnits)
            if (unit.IsSelected)
                selectedEntities.Add(unit);

        int numSelected = selectedEntities.Count;
        if (numSelected == 0) return;

        Unit firstEntity = selectedEntities[0];

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

    public abstract void SetTarget(Entity target);

    public void Stop()
    {
        reachedDestination = true;
        targetPosition = transform.position;
        isMoving = false;
    }

    protected override void Initialize()
    {
        base.Initialize();
        currentMana = Data.maxManaPoints;
        attackSpeed = Data.attackSpeed;
        movementSpeed = Data.movementSpeed;
        manaBar?.Initialize(Data.maxManaPoints);
    }


    private Vector3 AvoidCollisions()
    {
        List<Unit> neighbors = spatialGrid.GetNeighbors(transform.position);
        NativeArray<Vector3> unitPositions = new(neighbors.Count, Allocator.TempJob);
        for (int i = 0; i < neighbors.Count; i++) unitPositions[i] = neighbors[i].transform.position;

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
        SignalDeath();
        spatialGrid.Remove(this);
        UnitFactory.ReturnEntity(this);
    }

    public int GetManaPoints()
    {
        return currentMana;
    }

    public void SetManaPoints(int currentManaPoints)
    {
        currentMana = currentManaPoints;
        if (currentMana > Data.maxManaPoints)
            currentMana = Data.maxManaPoints;
        else if (currentMana < 0) currentMana = 0;
        manaBar.SetValue(currentMana);
    }

    public int GetMaxManaPoints()
    {
        return Data.maxManaPoints;
    }
}