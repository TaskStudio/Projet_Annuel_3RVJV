using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Unit : NewEntity, IMovable
{
    public float avoidanceStrength = 5f;
    public LayerMask Entity;

    public float moveSpeed = 5f;
    [SerializeField] private Profile profile;
    public float stoppingDistance;
    protected float collisionRadius = 1f;
    private Collider entityCollider;

    protected bool isMoving;
    private Vector3 targetPosition;

    protected void Start()
    {
        EntitiesManager.Instance.RegisterMovableEntity(this);
        entityCollider = GetComponent<Collider>();
        targetPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null) EntitiesManager.Instance.UnregisterMovableEntity(this);

        if (SelectionManager.Instance != null) SelectionManager.Instance.DeselectEntity(this);
    }

    public void Move(Vector3 newPosition)
    {
        targetPosition = newPosition;
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
}