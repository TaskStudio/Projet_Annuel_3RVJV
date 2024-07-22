using System.Collections;
using UnityEngine;

public class Enemy : Fighter
{
    protected float bumpDistance = 1f;
    protected int collisionDamage = 20;

    protected State currentState = State.Idle;
    protected bool isTaunted;
    protected Transform target;
    protected Vector3 tauntTarget;

    protected virtual void Start()
    {
        base.Start();
        if (!mapEditContext)
            StartCoroutine(BehaviorTree());
    }

    protected override void Die()
    {
        base.Die();
        StatManager.IncrementEnemyDeathCount();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        StatManager.IncrementEnemyDamageTaken(damage);
    }

    protected IEnumerator BehaviorTree()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Idle:
                    FindTarget();
                    break;
                case State.MovingToTarget:
                    MoveToTarget();
                    break;
                case State.Attacking:
                    AttackTarget();
                    break;
            }

            yield return null;
        }
    }

    protected void FindTarget()
    {
        float detectionRadius = Mathf.Infinity;
        LayerMask allyLayer = LayerMask.GetMask("Ally");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, allyLayer);

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        GameObject closestTarget = null;

        // Iterate through the colliders to find the closest ally
        foreach (Collider hitCollider in hitColliders)
        {
            float dSqrToTarget = (hitCollider.transform.position - currentPosition).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = hitCollider.gameObject;
            }
        }

        // Set the target and update the state accordingly
        if (closestTarget != null)
        {
            target = closestTarget.transform;
            currentState = State.MovingToTarget;
        }
        else
        {
            currentState = State.Idle;
        }
    }


    protected void MoveToTarget()
    {
        if (target != null)
        {
            targetPosition = target.position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                movementSpeed * Time.deltaTime
            );

            // Check if close enough to attack
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < Data.attackRange) currentState = State.Attacking;
        }
        else
        {
            currentState = State.Idle;
            FindTarget();
        }
    }

    protected virtual void AttackTarget()
    {
        if (target != null)
        {
            Attack();

            // Apply bump effect on the enemy
            Vector3 bumpDirection = (transform.position - target.position).normalized;
            Vector3 bumpPosition = transform.position + bumpDirection * bumpDistance;

            // Ensure the enemy stays on the same Y level (ground level)
            bumpPosition.y = transform.position.y;

            transform.position = bumpPosition;

            currentState = State.Idle;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    public virtual void Move(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if (targetPosition != transform.position)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * movementSpeed);
        }
    }

    public override void SetTarget(Entity target)
    {
        if (target == null) return;
        if (target is Ally) currentTarget = target;
    }

    public virtual void Taunt(Tank taunter)
    {
        tauntTarget = taunter.transform.position;
        isTaunted = true;
    }

    protected enum State
    {
        Idle,
        MovingToTarget,
        Attacking
    }
}