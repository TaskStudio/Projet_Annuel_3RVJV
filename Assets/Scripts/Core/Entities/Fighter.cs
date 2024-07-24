using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class Fighter : Unit
{
    public enum DistanceType
    {
        Melee,
        Ranged
    }

    [Space(10)] [Header("Fighting")] [SerializeField]
    protected LayerMask targetLayer;
    [SerializeField] [CanBeNull] private string attackingBool;

    [SerializeField] private ParticleSystem attackParticleSystem;

    protected readonly Collider[] potentialTargetsInRange = new Collider[50];
    private readonly HashSet<Entity> targetsHashSet = new();

    protected Entity currentTarget;
    protected Vector3 heldPosition;
    private float lastAttackTime;
    protected bool moveAttack;
    protected List<Entity> targetsInRange = new();


    protected new void Start()
    {
        base.Start();
        lastAttackTime = -Data.attackCooldown;
        heldPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        var numTargets = Physics.OverlapSphereNonAlloc(
            transform.position,
            Data.detectionRange,
            potentialTargetsInRange,
            targetLayer
        );

        for (var i = 0; i < numTargets; i++)
            if (colliderToEntityMap.TryGetValue(potentialTargetsInRange[i], out var potentialTarget)
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


    protected virtual void Attack()
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
            if (animator != null) animator.SetBool(attackingBool, false);
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
        if (Data.distanceType == DistanceType.Ranged && attackParticleSystem != null)
        {
            // Instantiate the particle system at the current position

            attackParticleSystem.transform.LookAt(
                currentTarget.transform
            ); // Ensure the particles are directed towards the target
            attackParticleSystem.Play();
        }

        if (animator != null) animator.SetBool(attackingBool, true);
        currentTarget.TakeDamage(Data.attackDamage);

        lastAttackTime = Time.time;
    }

    private Entity GetNearestTarget()
    {
        Entity nearestTarget = null;
        var nearestDistance = float.MaxValue;
        foreach (var target in targetsInRange)
        {
            var distance = Vector3.Distance(transform.position, target.transform.position);
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

    public override void TargetIsDead(Entity target)
    {
        if (currentTarget == target) currentTarget = null;
        if (targetsHashSet.Remove(target)) targetsInRange.Remove(target);

        targetPosition = heldPosition;
    }
}