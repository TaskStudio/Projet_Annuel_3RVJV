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
        StartCoroutine(BehaviorTree());
    }

    protected new void Update()
    {
        base.Update();
    }

    protected IEnumerator BehaviorTree()
    {
        while (true)
        {
            Debug.Log("Current State: " + currentState);
            switch (currentState)
            {
                case State.Idle:
                    FindTarget();
                    break;
                case State.MovingToTarget:
                    MoveToTarget();
                    break;
                case State.Attacking:
                    Debug.Log("Attacking state triggered");
                    AttackTarget();
                    break;
            }

            yield return null; // Wait for the next frame
        }
    }

    protected void FindTarget()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        GameObject[] entityBases = GameObject.FindGameObjectsWithTag("EntityBase");
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        GameObject closestTarget = null;

        foreach (GameObject entity in entities)
        {
            float dSqrToTarget = (entity.transform.position - currentPosition).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = entity;
            }
        }

        if (closestTarget == null)
        {
            foreach (GameObject entityBase in entityBases)
            {
                float dSqrToTarget = (entityBase.transform.position - currentPosition).sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = entityBase;
                }
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget.transform;
            currentState = State.MovingToTarget;
            Debug.Log("New target acquired: " + target.name);
        }
        else
        {
            Debug.LogWarning("No target found, remaining in Idle state");
            currentState = State.Idle;
        }
    }

    protected void MoveToTarget()
    {
        if (target != null)
        {
            targetPosition = target.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        
            // Check if close enough to attack
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            Debug.Log("Distance to target: " + distanceToTarget);
            if (distanceToTarget < 1.0f) // Adjust the distance threshold as needed
            {
                Debug.Log("Reached target, switching to Attacking state");
                currentState = State.Attacking;
            }
            else
            {
                Debug.Log("Current State: MovingToTarget, Distance to Target: " + distanceToTarget);
            }
        }
        else
        {
            Debug.LogWarning("Target is null during movement, switching to Idle state");
            currentState = State.Idle;
            FindTarget(); // Try to find a new target immediately
        }
    }

    protected virtual void AttackTarget()
    {
        if (target != null)
        {
            Debug.Log("Calling Fighter's Attack method");
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
            Debug.LogWarning("Target is null in AttackTarget, switching to Idle state");
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

    protected Vector3 FindNearestTarget()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        GameObject[] entityBases = GameObject.FindGameObjectsWithTag("EntityBase");
        Vector3 currentPosition = transform.position;

        GameObject closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject entity in entities)
        {
            Vector3 directionToTarget = entity.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = entity;
            }
        }

        if (closestTarget == null && entityBases.Length > 0)
            foreach (GameObject entityBase in entityBases)
            {
                Vector3 directionToTarget = entityBase.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = entityBase;
                }
            }

        return closestTarget != null ? closestTarget.transform.position : Vector3.positiveInfinity;
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
