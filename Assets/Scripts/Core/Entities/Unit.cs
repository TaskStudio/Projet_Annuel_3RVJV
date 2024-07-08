using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Unit : Entity
{
    public float stoppingDistance = 0.5f;
    public LayerMask Entity;
    public float collisionRadius = 1f;
    public float avoidanceStrength = 5f;
    private Vector3 targetPosition;
    private Vector3 originalTargetPosition; // Store the original target position
    private Collider entityCollider;
    private bool isMoving = false;
    private bool needsCollisionAvoidance = false;
    private bool isInFormation = false; // Track if the unit is moving in formation

    protected void Start()
    {
        base.Start();
        EntitiesManager.Instance.RegisterMovableEntity(this);
        targetPosition = transform.position;
        originalTargetPosition = transform.position; // Initialize original target position
    }

    protected virtual void Update()
    {
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosition.x, targetPosition.z));

        if (distanceToTarget > stoppingDistance || !isMoving)
        {
            if (!isMoving)
            {
                isMoving = true;
            }
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
        }
        else if (distanceToTarget <= stoppingDistance && isMoving)
        {
            isMoving = false;
            targetPosition = transform.position;
            needsCollisionAvoidance = true;
        }

        // Ensure entity returns to the original target position if displaced
        if (needsCollisionAvoidance && Vector3.Distance(transform.position, originalTargetPosition) > stoppingDistance)
        {
            Move(originalTargetPosition);
        }

        // Handle formation movement
        if (isInFormation)
        {
            HandleFormationMovement();
        }
    }

    public void Move(Vector3 newPosition)
    {
        if (this != null)
        {
            targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.z); // Keep y position the same
            originalTargetPosition = targetPosition; // Update original target position
            isMoving = true;
            needsCollisionAvoidance = false;
            isInFormation = false; // Reset formation flag
        }
    }

    public void MoveInFormation(Vector3 targetPosition)
    {
        List<Unit> selectedEntities = new List<Unit>();

        foreach (var Unit in EntitiesManager.MovableEntities)
        {
            if (Unit is BaseObject selectable && selectable.IsSelected)
            {
                selectedEntities.Add(Unit);
            }
        }

        int numSelected = selectedEntities.Count;
        if (numSelected == 0)
        {
            return;
        }

        Unit firstEntity = selectedEntities[0] as Unit;
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
        float totalHeight = Mathf.CeilToInt((float)numSelected / entitiesPerRow) * spacing;

        Vector3 topLeftPosition = targetPosition - new Vector3(totalWidth / 2, 0, totalHeight / 2);

        for (int i = 0; i < selectedEntities.Count; i++)
        {
            int row = i / entitiesPerRow;
            int column = i % entitiesPerRow;

            Vector3 offsetPosition = new Vector3(column * spacing, 0, row * spacing);
            selectedEntities[i].Move(topLeftPosition + offsetPosition);
            selectedEntities[i].isInFormation = true; // Set formation flag
        }
    }

    private void HandleFormationMovement()
    {
        // Ensure entity moves to the original target position while avoiding collisions
        Vector3 adjustedPosition = AvoidCollisions();
        MoveTowardsTarget(adjustedPosition);

        // Stop formation flag if the entity has reached its target position
        if (Vector3.Distance(transform.position, originalTargetPosition) <= stoppingDistance)
        {
            isInFormation = false;
        }
    }

    protected Vector3 AvoidCollisions()
    {
        if (!needsCollisionAvoidance && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosition.x, targetPosition.z)) <= stoppingDistance)
        {
            return targetPosition;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collisionRadius, Entity);
        Vector3 avoidanceVector = Vector3.zero;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != entityCollider)
            {
                Vector3 collisionDirection = transform.position - hitCollider.transform.position;
                avoidanceVector += collisionDirection.normalized;
            }
        }

        if (avoidanceVector != Vector3.zero)
        {
            avoidanceVector = avoidanceVector.normalized * avoidanceStrength;
        }

        return targetPosition + avoidanceVector;
    }

    protected void MoveTowardsTarget(Vector3 adjustedPosition)
    {
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(adjustedPosition.x, adjustedPosition.z));
        if (distanceToTarget <= stoppingDistance)
        {
            return;
        }

        NativeArray<Vector3> newPositionArray = new NativeArray<Vector3>(1, Allocator.TempJob);

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
