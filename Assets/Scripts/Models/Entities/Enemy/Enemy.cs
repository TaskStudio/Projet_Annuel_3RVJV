using System.Collections;
using FogOfWar;
using UnityEngine;

public class Enemy : Fighter
{
    protected float bumpDistance = 1f;
    protected int collisionDamage = 20;

    protected State currentState = State.Idle;

    private FogScript fogScript;
    protected bool isTaunted;
    protected Transform target;
    protected Vector3 tauntTarget;

    protected virtual void Start()
    {
        base.Start();
        if (!mapEditContext)
            StartCoroutine(BehaviorTree());

        fogScript = FindObjectOfType<FogScript>(); // Ensure there is only one FogScript in the scene
    }

    private void Update()
    {
        if (fogScript != null)
        {
            var isVisible = fogScript.IsPositionInClearedZone(transform.position);
            SetVisibility(!isVisible);
        }
    }

    private void SetVisibility(bool isVisible)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>()) renderer.enabled = isVisible;
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
        var detectionRadius = Mathf.Infinity;
        LayerMask allyLayer = LayerMask.GetMask("Ally");

        var hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, allyLayer);

        var closestDistanceSqr = Mathf.Infinity;
        var currentPosition = transform.position;
        GameObject closestTarget = null;

        // Iterate through the colliders to find the closest ally
        foreach (var hitCollider in hitColliders)
        {
            var dSqrToTarget = (hitCollider.transform.position - currentPosition).sqrMagnitude;
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
            var distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < Data.attackRange)
            {
                Stop();
                currentState = State.Attacking;
            }
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
            var bumpDirection = (transform.position - target.position).normalized;
            var bumpPosition = transform.position + bumpDirection * bumpDistance;

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
            var direction = (targetPosition - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
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