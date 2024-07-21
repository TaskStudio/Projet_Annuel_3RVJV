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

        if (targetsInRange.Count > 0 || currentTarget != null) Attack();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
        {
            Unit entity = collision.gameObject.GetComponent<Unit>();
            if (entity != null)
            {
                entity.TakeDamage(collisionDamage);

                Vector3 bumpDirection = (transform.position - collision.transform.position).normalized;
                transform.position += bumpDirection * bumpDistance;
            }
        }

        if (collision.gameObject.CompareTag("EntityBase"))
        {
            EntityBases entityBase = collision.gameObject.GetComponent<EntityBases>();
            if (entityBase != null) entityBase.TakeDamage(1000);
            Destroy(gameObject);
        }
    }
    
    protected override void Die()
    {
        base.Die();
        KillCounter.IncrementEnemyDeathCount();
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
                case State.BackingUp:
                    Backup();
                    break;
                case State.Defending:
                    Defend();
                    break;
                case State.SeekingBackup:
                    SeekBackup();
                    break;
            }

            yield return null;
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
            foreach (GameObject entityBase in entityBases)
            {
                float dSqrToTarget = (entityBase.transform.position - currentPosition).sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = entityBase;
                }
            }

        if (closestTarget != null)
        {
            target = closestTarget.transform;
            currentState = State.MovingToTarget;
        }
    }

    protected void MoveToTarget()
    {
        if (target != null)
        {
            // transform.position = Vector3.MoveTowards(
            //     transform.position,
            //     target.position,
            //     movementSpeed * Time.deltaTime
            // );
            targetPosition = target.position;
            if (Vector3.Distance(transform.position, target.position) < 0.5f) currentState = State.Attacking;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    protected virtual void AttackTarget()
    {
        if (target != null)
        {
            Unit entity = target.GetComponent<Unit>();
            if (entity != null) entity.TakeDamage(20);
            currentState = State.Idle;
        }
    }

    protected virtual void Backup()
    {
        Vector3 directionAwayFromTarget = (transform.position - target.position).normalized;
        Vector3 backupPosition = transform.position + directionAwayFromTarget * bumpDistance;

        transform.position = Vector3.MoveTowards(transform.position, backupPosition, movementSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, backupPosition) < 0.1f) currentState = State.SeekingBackup;
    }

    protected virtual void Defend()
    {
        StartCoroutine(DefenseCoroutine());
    }

    protected IEnumerator DefenseCoroutine()
    {
        yield return new WaitForSeconds(3f);
        currentState = State.Idle;
    }

    protected virtual void SeekBackup()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        GameObject closestAlly = null;

        foreach (GameObject ally in allies)
            if (ally != gameObject)
            {
                float dSqrToTarget = (ally.transform.position - currentPosition).sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestAlly = ally;
                }
            }

        if (closestAlly != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                closestAlly.transform.position,
                movementSpeed * Time.deltaTime
            );
            if (Vector3.Distance(transform.position, closestAlly.transform.position) < 0.5f)
                currentState = State.Defending;
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
        Attacking,
        BackingUp,
        Defending,
        SeekingBackup
    }
}