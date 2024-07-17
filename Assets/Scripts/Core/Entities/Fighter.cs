using System.Collections.Generic;
using UnityEngine;

public abstract class Fighter : Unit<FighterData>
{
    public enum DistanceType
    {
        Melee,
        Ranged
    }

    [Space(10)] [Header("Combat")]
    [SerializeField] private SphereCollider detectionSphere;
    protected readonly List<IEntity> targetsInRange = new();

    protected IEntity currentTarget;
    private Vector3 heldPosition;
    private float lastAttackTime;
    protected bool moveAttack;


    protected new void Start()
    {
        base.Start();
        lastAttackTime = -data.attackCooldown;
        detectionSphere.radius = data.detectionRange;
        heldPosition = transform.position;
    }


    public override void Move(Vector3 newPosition)
    {
        base.Move(newPosition);
        heldPosition = newPosition;
        currentTarget = null;
    }


    protected void Attack()
    {
        if (currentTarget == null) currentTarget = GetNearestTarget();

        if (currentTarget == null)
        {
            targetPosition = heldPosition;
            return;
        }


        var targetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (targetDistance > data.attackRange)
        {
            targetPosition = currentTarget.transform.position;
            return;
        }

        Stop();
        if (moveAttack)
        {
            moveAttack = false;
            heldPosition = transform.position;
        }

        gameObject.transform.LookAt(currentTarget.transform.position);

        if (Time.time <= lastAttackTime + data.attackCooldown) return;
        currentTarget.TakeDamage(data.attackDamage);
        lastAttackTime = Time.time;
    }

    public abstract void AddTargetInRange(IEntity target);

    public abstract void RemoveTargetInRange(IEntity target);

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

    public override void TargetIsDead(IEntity entity)
    {
        if (currentTarget == entity) currentTarget = null;
        if (targetsInRange.Contains(entity)) targetsInRange.Remove(entity);
        targetPosition = heldPosition;
    }
}