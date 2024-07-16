using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit<FighterData>
{
    public enum DistanceType
    {
        Melee,
        Ranged
    }

    [Space(10)] [Header("Combat")]
    [SerializeField] private DistanceType distanceType;
    [SerializeField] private SphereCollider detectionSphere;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float chaseDistance = 15f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    private readonly List<IEntity> targetsInRange = new();

    private IEntity currentTarget;
    private Vector3 heldPosition;
    private float lastAttackTime;

    protected void Start()
    {
        base.Start();
        lastAttackTime = -attackCooldown;
        detectionSphere.radius = detectionRange;
        heldPosition = transform.position;
    }

    protected new void Update()
    {
        base.Update();
        if (targetsInRange.Count > 0) Attack();
    }

    public override void Move(Vector3 newPosition)
    {
        base.Move(newPosition);
        heldPosition = newPosition;
    }

    private void Attack()
    {
        if (currentTarget == null) currentTarget = GetNearestTarget();

        var targetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (targetDistance > attackRange)
        {
            targetPosition = currentTarget.transform.position;
            return;
        }

        Stop();
        gameObject.transform.LookAt(currentTarget.transform.position);

        if (Time.time <= lastAttackTime + attackCooldown) return;
        currentTarget.TakeDamage(data.attackDamage);
        lastAttackTime = Time.time;
        if (currentTarget.IsDead())
        {
            targetsInRange.Remove(currentTarget);
            currentTarget = null;
            targetPosition = heldPosition;
        }
    }

    public void AddTargetInRange(IEntity target)
    {
        targetsInRange.Add(target);
    }

    public void RemoveTargetInRange(IEntity target)
    {
        targetsInRange.Remove(target);
    }

    private IEntity GetNearestTarget()
    {
        IEntity nearestTarget = null;
        float nearestDistance = float.MaxValue;
        foreach (IEntity target in targetsInRange)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }
}