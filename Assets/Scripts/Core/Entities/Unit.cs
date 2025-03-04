using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Allocator = Unity.Collections.Allocator;

[Serializable]
public abstract class Unit : Entity
{
    protected static SpatialGrid spatialGrid;

    private static Collider[] potentialColliders = new Collider[50];
    private static NativeArray<Vector3> unitPositions;
    private static NativeArray<Vector3> avoidanceVectorArray;

    [Space(10)] [Header("Movement")]
    [SerializeField] protected float avoidanceStrength = 5f;
    [SerializeField] protected float collisionRadius = 1f;

    [Space(10)] [Header("Mana")]
    [SerializeField] private ManaBar manaBar;

    [Space(10)] [Header("Animation")]
    [SerializeField] [CanBeNull] private string walkingBool;
    [SerializeField] [CanBeNull] protected Animator animator;

    private Vector3 avoidanceVector;
    private bool isMoving;
    private Vector3 lastPosition;

    private bool needsCollisionAvoidance;
    private Vector3 originalTargetPosition;
    protected bool reachedDestination;
    protected float stoppingDistance = 0.1f;
    protected Vector3 targetPosition;

    private Unit unitImplementation;

    public int currentMana { get; protected set; }
    public float movementSpeed { get; protected set; } = 0.5f;
    public float attackSpeed { get; protected set; } = 1.0f;

    public float CollisionRadius => collisionRadius;

    protected void Start()
    {
        UnitsManager.Instance.RegisterMovableEntity(this);
        if (mapEditContext) return;
        if (spatialGrid == null) spatialGrid = new SpatialGrid(5f);

        spatialGrid.Add(this);
        lastPosition = transform.position;

        targetPosition = transform.position;
        originalTargetPosition = transform.position;

        unitPositions = new NativeArray<Vector3>(50, Allocator.Persistent);
        avoidanceVectorArray = new NativeArray<Vector3>(1, Allocator.Persistent);
    }

    protected virtual void Update()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy || mapEditContext)
            return; // Early exit if the unit is destroyed or inactive

        spatialGrid.Update(this, lastPosition);
        lastPosition = transform.position;

        float distanceToTarget = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(targetPosition.x, targetPosition.z)
        );

        if (distanceToTarget > stoppingDistance)
        {
            transform.LookAt(targetPosition);
            if (animator && !animator.GetBool(walkingBool)) animator.SetBool(walkingBool, true);
            if (!isMoving) isMoving = true;
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
        }
        else if (distanceToTarget <= stoppingDistance && isMoving)
        {
            if (animator && animator.GetBool(walkingBool)) animator.SetBool(walkingBool, false);
            Stop();
            needsCollisionAvoidance = true;
        }

        if (needsCollisionAvoidance
            && !reachedDestination
            && Vector3.Distance(transform.position, originalTargetPosition) > stoppingDistance)
        {
            // animator.SetBool(isWalking, true);
            Move(originalTargetPosition);
            needsCollisionAvoidance = false;
        }

        avoidanceVector = Vector3.zero;
    }

    private void OnDestroy()
    {
        if (unitPositions.IsCreated)
            unitPositions.Dispose();
        if (avoidanceVectorArray.IsCreated)
            avoidanceVectorArray.Dispose();
    }

    private Vector3 AvoidCollisions()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, collisionRadius, potentialColliders);
        if (!unitPositions.IsCreated || numColliders > unitPositions.Length)
        {
            if (unitPositions.IsCreated) unitPositions.Dispose();
            unitPositions = new NativeArray<Vector3>(numColliders, Allocator.Persistent);
        }

        if (!avoidanceVectorArray.IsCreated) avoidanceVectorArray = new NativeArray<Vector3>(1, Allocator.Persistent);

        for (int i = 0; i < numColliders; i++) unitPositions[i] = potentialColliders[i].transform.position;

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

        return targetPosition + avoidanceVectorArray[0];
    }


    public new void SignalDeath()
    {
        base.SignalDeath();

        // Collect units to remove in a separate list
        var unitsToRemove = new List<Unit>(targetedBy);
        foreach (Unit unit in unitsToRemove)
        {
            if (unit != null)
                unit.RemoveTargetedBy(this);
        }

        UnitsManager.Instance.UnregisterMovableEntity(this);
    }


    public abstract void TargetIsDead(Entity target);

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