using System.Collections.Generic;
using UnityEngine;

public abstract class Fighter : Unit
{
    public enum DistanceType
    {
        Melee,
        Ranged
    }

    [Space(10)] [Header("Fighting")]
    [SerializeField] protected LayerMask targetLayer;

    protected readonly Collider[] potentialTargetsInRange = new Collider[50];
    private readonly HashSet<Unit> targetsHashSet = new();

    protected Entity currentTarget;
    protected Vector3 heldPosition;
    private float lastAttackTime;
    protected bool moveAttack;
    protected List<Unit> targetsInRange = new();


    protected new void Start()
    {
        base.Start();
        lastAttackTime = -Data.attackCooldown;
        heldPosition = transform.position;
    }

    protected new void Update()
    {
        base.Update();

        int numTargets = Physics.OverlapSphereNonAlloc(
            transform.position,
            Data.detectionRange,
            potentialTargetsInRange,
            targetLayer
        );

        for (var i = 0; i < numTargets; i++)
            if (colliderToUnitMap.TryGetValue(potentialTargetsInRange[i], out Unit potentialTarget)
                && targetsHashSet.Add(potentialTarget))
            {
                targetsInRange.Add(potentialTarget);
                potentialTarget.AddTargetedBy(this);
            }
    }

    public override void Move(Vector3 newPosition)
    {
        base.Move(newPosition);
        heldPosition = newPosition;
        currentTarget = null;
    }

    public override void MoveInFormation(Vector3 targetPosition)
    {
        base.MoveInFormation(targetPosition);
        heldPosition = targetPosition;
        currentTarget = null;
    }


    protected void Attack()
    {
        currentTarget ??= GetNearestTarget();

        if (currentTarget == null)
        {
            targetPosition = heldPosition;
            return;
        }


        var targetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (targetDistance > Data.attackRange)
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

        if (Time.time <= lastAttackTime + Data.attackCooldown) return;
        currentTarget.TakeDamage(Data.attackDamage);
        lastAttackTime = Time.time;
    }

    private Unit GetNearestTarget()
    {
        Unit nearestTarget = null;
        float nearestDistance = float.MaxValue;
        foreach (Unit target in targetsInRange)
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

    public void MoveAndAttack(Vector3 targetFormationPosition)
    {
        MoveInFormation(targetFormationPosition);
        moveAttack = true;
    }

    public override void TargetIsDead(Unit unit)
    {
        if (currentTarget == unit) currentTarget = null;
        if (targetsHashSet.Remove(unit)) targetsInRange.Remove(unit);

        targetPosition = heldPosition;
    }
}