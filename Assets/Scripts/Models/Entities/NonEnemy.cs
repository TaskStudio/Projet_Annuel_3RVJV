using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;

public class NonEnemy : Entity, IMovable, IShootable, ISelectable
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject selectionIndicatorPrefab;
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.5f;
    public LayerMask Entity;
    public float collisionRadius = 1f;
    public float avoidanceStrength = 5f;
    public bool IsSelected { get; set; }

    private EntityVisuals visuals;
    private Vector3 targetPosition;
    private Collider entityCollider;
    private bool isMoving = false;

    protected void Start()
    {
        EntitiesManager.Instance.RegisterMovableEntity(this);
        visuals = GetComponent<EntityVisuals>();
        entityCollider = GetComponent<Collider>();
        if (visuals == null)
        {
            visuals = gameObject.AddComponent<EntityVisuals>();
        }
        visuals.selectionIndicatorPrefab = selectionIndicatorPrefab;
        targetPosition = transform.position;
    }

    protected virtual void Update()
    {
        HandleInput();

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
            targetPosition = transform.position; // Ensure the entity stops moving
        }

        // Check if entity is pushed away from the target position and move it back
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Move(targetPosition);
        }
    }

    protected virtual void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && IsSelected)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Shoot(mousePosition);
        }
    }

    public void Move(Vector3 newPosition)
    {
        if (this != null)
        {
            targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.z); // Keep y position the same
            isMoving = true;
        }
    }

    public void MoveInFormation(Vector3 targetPosition)
    {
        List<IMovable> selectedEntities = new List<IMovable>();

        foreach (var entity in EntitiesManager.MovableEntities)
        {
            if (entity is ISelectable selectable && selectable.IsSelected)
            {
                selectedEntities.Add(entity);
            }
        }

        int numSelected = selectedEntities.Count;
        if (numSelected == 0)
        {
            return;
        }

        NonEnemy firstEntity = selectedEntities[0] as NonEnemy;
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
        }
    }

    protected Vector3 AvoidCollisions()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosition.x, targetPosition.z)) <= stoppingDistance)
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
            moveSpeed = moveSpeed,
            deltaTime = Time.deltaTime,
            newPosition = newPositionArray
        };

        JobHandle moveJobHandle = moveJob.Schedule();
        moveJobHandle.Complete();

        Vector3 newPosition = newPositionArray[0];
        newPositionArray.Dispose();

        transform.position = newPosition;
    }

    public virtual void Shoot(Vector3 target)
    {
        if (projectilePrefab && projectileSpawnPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                hitPoint.y = projectileSpawnPoint.position.y;
                Vector3 shootDirection = (hitPoint - projectileSpawnPoint.position).normalized;

                projectile.GetComponent<Projectile>().Initialize(shootDirection, 20f);
            }
        }
    }

    public void Select()
    {
        IsSelected = true;
        UpdateVisuals();
    }

    public void Deselect()
    {
        IsSelected = false;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (visuals)
        {
            visuals.UpdateVisuals(IsSelected);
        }
    }

    public IProfile GetProfile()
    {
        return null;
    }
}
