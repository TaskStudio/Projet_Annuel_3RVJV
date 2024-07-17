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
    [SerializeField] private SphereCollider detectionSphere;
    private readonly List<IEntity> targetsInRange = new();

    protected IEntity currentTarget;
    private Vector3 heldPosition;
    private float lastAttackTime;

    protected new void Start()
    {
        base.Start();
        lastAttackTime = -data.attackCooldown;
        detectionSphere.radius = data.detectionRange;
        heldPosition = transform.position;
    }

    protected new void Update()
    {
        base.Update();
        if (reachedDestination && (targetsInRange.Count > 0 || currentTarget != null)) Attack();
    }

    public override void Move(Vector3 newPosition)
    {
        base.Move(newPosition);
        heldPosition = newPosition;
        currentTarget = null;
    }

    public override void SetTarget(IEntity target)
    {
        if (target == null) return;
        if (target is Enemy) currentTarget = target;
    }

    private void Attack()
    {
        if (currentTarget == null) currentTarget = GetNearestTarget();

        var targetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (targetDistance > data.attackRange)
        {
            targetPosition = currentTarget.transform.position;
            return;
        }

        Stop();
        gameObject.transform.LookAt(currentTarget.transform.position);

        if (Time.time <= lastAttackTime + data.attackCooldown) return;
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