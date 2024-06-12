using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

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
        //Debug.Log($"Entity {name} distance to target (x and z): {distanceToTarget}");

        if (distanceToTarget > stoppingDistance)
        {
            if (!isMoving)
            {
                isMoving = true;
                //Debug.Log($"Entity {name} started moving towards {targetPosition}");
            }
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                //Debug.Log($"Entity {name} has stopped moving at {transform.position}");
            }
            else
            {
                //Debug.Log($"Entity {name} is not moving and is at position {transform.position}");
            }
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
            //Debug.Log($"Entity {name} received move command to {newPosition}");
            targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.z); // Keep y position the same
            isMoving = true;
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
            //Debug.Log($"Entity {name} is within stopping distance: {distanceToTarget}");
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

        //Debug.Log($"Entity {name} moved from {transform.position} to {newPosition}");

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
