using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Unit : Entity<UnitData>
{
    [Space(10)] [Header("Movement")]
    [SerializeField] protected LayerMask entityLayer;
    [SerializeField] protected float collisionRadius = 1f;
    [SerializeField] protected float avoidanceStrength = 5f;

    private Collider entityCollider;
    private bool isMoving;
    protected float stoppingDistance = 0.5f;
    private Vector3 targetPosition;

    public int currentMana { get; private set; }
    public float movementSpeed { get; protected set; } = 0.5f;
    public float attackSpeed { get; private set; } = 1.0f;

    protected void Start()
    {
        EntitiesManager.Instance.RegisterMovableEntity(this);
        targetPosition = transform.position;
    }

    protected virtual void Update()
    {
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
            targetPosition = transform.position; // Ensure the entity stops moving
        }

        // Check if entity is pushed away from the target position and move it back
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) Move(targetPosition);
    }

    protected override void Initialize()
    {
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
            ); // Keep y position the same
            isMoving = true;
        }
    }

    public void MoveInFormation(Vector3 targetPosition)
    {
        List<Unit> selectedEntities = new();

        foreach (var Unit in EntitiesManager.MovableEntities)
            if (Unit is Unit selectable && selectable.IsSelected)
                selectedEntities.Add(Unit);

        int numSelected = selectedEntities.Count;
        if (numSelected == 0) return;

        Unit firstEntity = selectedEntities[0];
        if (firstEntity == null)
        {
            Debug.LogError("Entities must be of type NonEnemy to calculate their collision radius.");
            return;
        }

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
        if (Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(targetPosition.x, targetPosition.z)
            )
            <= stoppingDistance) return targetPosition;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collisionRadius, entityLayer);
        Vector3 avoidanceVector = Vector3.zero;

        foreach (var hitCollider in hitColliders)
            if (hitCollider != entityCollider)
            {
                Vector3 collisionDirection = transform.position - hitCollider.transform.position;
                avoidanceVector += collisionDirection.normalized;
            }

        if (avoidanceVector != Vector3.zero) avoidanceVector = avoidanceVector.normalized * avoidanceStrength;

        return targetPosition + avoidanceVector;
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
}