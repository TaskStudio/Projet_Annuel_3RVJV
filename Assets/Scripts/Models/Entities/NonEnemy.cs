using Managers.Entities;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class NonEnemy : Entity, IMovable, IShootable, ISelectable
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject selectionIndicatorPrefab;
    public float moveSpeed = 5f;
    public float stoppingDistance;
    public LayerMask Entity;
    public float collisionRadius = 1f;
    public float avoidanceStrength = 5f;
    private Collider entityCollider;
    private Vector3 targetPosition;

    private EntityVisuals visuals;

    protected void Start()
    {
        EntitiesManager.Instance.RegisterMovableEntity(this);
        visuals = GetComponent<EntityVisuals>();
        entityCollider = GetComponent<Collider>();
        if (visuals == null) visuals = gameObject.AddComponent<EntityVisuals>();
        visuals.selectionIndicatorPrefab = selectionIndicatorPrefab;
        targetPosition = transform.position;
    }

    protected virtual void Update()
    {
        HandleInput();
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
        }
    }

    public void Move(Vector3 newPosition)
    {
        if (this != null)
        {
            // Notify the spawner to free the old position
            EntitySpawner spawner = FindObjectOfType<EntitySpawner>();
            if (spawner != null) spawner.FreePosition(transform.position);

            targetPosition = newPosition;
        }
    }

    public bool IsSelected { get; set; }

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

    public IProfile GetProfile()
    {
        return null;
    }

    public virtual void Shoot(Vector3 target)
    {
        if (projectilePrefab && projectileSpawnPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            Ray ray = Camera.main.ScreenPointToRay(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
            );
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                hitPoint.y = projectileSpawnPoint.position.y; // Adjust y to be at the spawn point's height if needed
                Vector3 shootDirection = (hitPoint - projectileSpawnPoint.position).normalized;

                // Initialize the projectile with direction and speed
                projectile.GetComponent<Projectile>().Initialize(shootDirection, 20f); // Set speed as necessary
            }
        }
    }

    protected virtual void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && IsSelected)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
            );
            Shoot(mousePosition);
        }
    }


    protected Vector3 AvoidCollisions()
    {
        if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance) return targetPosition;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collisionRadius, Entity);
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

    protected void MoveTowardsTarget(Vector3 adjustedPosition)
    {
        if (Vector3.Distance(transform.position, adjustedPosition) <= stoppingDistance) return;

        NativeArray<Vector3> newPositionArray = new(1, Allocator.TempJob);

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

    public void UpdateVisuals()
    {
        if (visuals) visuals.UpdateVisuals(IsSelected);
    }
}