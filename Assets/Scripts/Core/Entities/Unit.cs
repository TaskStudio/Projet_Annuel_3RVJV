using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Unit : Entity<UnitData>
{
    private static SpatialGrid spatialGrid;

    [Space(10)] [Header("Movement")] [SerializeField]
    protected LayerMask entityLayer;

    [SerializeField] protected float collisionRadius = 1f;
    [SerializeField] protected float avoidanceStrength = 5f;

    [Space(10)] [Header("Mana")] [SerializeField]
    private ManaBar manaBar;

    private Vector3 avoidanceVector;

    private Collider entityCollider;
    private bool isMoving;
    private Vector3 lastPosition;
    private bool needsCollisionAvoidance;
    private Vector3 originalTargetPosition;
    protected float stoppingDistance = 0.01f;
    private Vector3 targetPosition;

    public int currentMana { get; private set; }
    public float movementSpeed { get; protected set; }
    public float attackSpeed { get; private set; }

    protected void Start()
    {
        if (spatialGrid == null) spatialGrid = new SpatialGrid(5f);

        spatialGrid.Add(this);
        lastPosition = transform.position;

        EntitiesManager.Instance.RegisterMovableEntity(this);
        targetPosition = transform.position;
        originalTargetPosition = transform.position;
        entityCollider = GetComponent<Collider>();

        InitializeMana();
    }

    protected virtual void Update()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
            return; // Early exit if the unit is destroyed or inactive

        spatialGrid.Update(this, lastPosition);
        lastPosition = transform.position;

        var distanceToTarget = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(targetPosition.x, targetPosition.z)
        );

        if (distanceToTarget > stoppingDistance || !isMoving)
        {
            if (!isMoving) isMoving = true;
            var adjustedPosition = AvoidCollisions();
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

        /*
         
        // C'est pour le test 
        
        RegenerateMana();

        // Check for space bar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseMana(4);
        }
        */
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetManaPoints(data.maxManaPoints);
        attackSpeed = data.attackSpeed;
        movementSpeed = data.movementSpeed;
    }

    private void InitializeMana()
    {
        if (data.maxManaPoints > 0)
        {
            currentMana = data.maxManaPoints;
            manaBar.Initialize(data.maxManaPoints);
        }
        else
        {
            if (manaBar != null) Destroy(manaBar.gameObject);
        }
    }

    private void RegenerateMana()
    {
        if (currentMana < data.maxManaPoints)
        {
            // Sebi j'te laisse la main bg
        }
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

        foreach (var Unit in EntitiesManager.MovableEntities)
            if (Unit is Unit selectable && selectable.IsSelected)
                selectedEntities.Add(Unit);

        var numSelected = selectedEntities.Count;
        if (numSelected == 0) return;

        var firstEntity = selectedEntities[0];

        var collisionRadius = firstEntity.collisionRadius;
        var offset = 0.1f;
        var spacing = collisionRadius * 1.8f + offset;

        var entitiesPerRow = Mathf.CeilToInt(Mathf.Sqrt(numSelected));
        var totalWidth = entitiesPerRow * spacing;
        var totalHeight = Mathf.CeilToInt((float)numSelected / entitiesPerRow) * spacing;

        var topLeftPosition = targetPosition - new Vector3(totalWidth / 2, 0, totalHeight / 2);

        for (var i = 0; i < selectedEntities.Count; i++)
        {
            var row = i / entitiesPerRow;
            var column = i % entitiesPerRow;

            var offsetPosition = new Vector3(column * spacing, 0, row * spacing);
            selectedEntities[i].Move(topLeftPosition + offsetPosition);
        }
    }

    private Vector3 AvoidCollisions()
    {
        var neighbors = spatialGrid.GetNeighbors(transform.position);
        var unitPositions = new NativeArray<Vector3>(neighbors.Count, Allocator.TempJob);
        for (var i = 0; i < neighbors.Count; i++)
            if (neighbors[i] != null && neighbors[i].gameObject.activeInHierarchy)
                unitPositions[i] = neighbors[i].transform.position;

        var avoidanceVectorArray = new NativeArray<Vector3>(1, Allocator.TempJob);

        var job = new AvoidCollisionsJob
        {
            unitPositions = unitPositions,
            currentPosition = transform.position,
            collisionRadius = collisionRadius,
            avoidanceStrength = avoidanceStrength,
            avoidanceVector = avoidanceVectorArray
        };

        var handle = job.Schedule();
        handle.Complete();

        var avoidance = avoidanceVectorArray[0];

        unitPositions.Dispose();
        avoidanceVectorArray.Dispose();

        return targetPosition + avoidance;
    }

    private void MoveTowardsTarget(Vector3 adjustedPosition)
    {
        var distanceToTarget = Vector2.Distance(
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

        var moveJobHandle = moveJob.Schedule();
        moveJobHandle.Complete();

        var newPosition = newPositionArray[0];
        newPositionArray.Dispose();

        transform.position = newPosition;
    }

    protected override void Die()
    {
        UnitFactory.ReturnEntity(this);
    }

    public int GetManaPoints()
    {
        return currentMana;
    }

    public void SetManaPoints(int currentManaPoints)
    {
        currentMana = currentManaPoints;
        if (currentMana > data.maxManaPoints)
            currentMana = data.maxManaPoints;
        else if (currentMana < 0) currentMana = 0;
        manaBar.SetValue(currentMana);
    }

    public int GetMaxManaPoints()
    {
        return data.maxManaPoints;
    }
    
    /*
    public void UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            manaBar.SetValue(currentMana);
            Debug.Log("Current Mana: " + currentMana);
        }
        else
        {
            Debug.Log("Not enough mana.");
        }
    }
    */
}