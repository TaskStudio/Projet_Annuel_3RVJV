using UnityEngine;

public class Enemy : Entity, IMovable
{
    public float moveSpeed = 5f;
    public int collisionDamage = 20;  // Damage dealt to other objects on collision
    private Vector3 tauntTarget;
    private bool isTaunted;

    void Update()
    {
        if (isTaunted)
        {
            Move(tauntTarget);
        }
        Vector3 moveTarget = isTaunted ? tauntTarget : FindNearestTarget();
        Move(moveTarget);
    }

    public void Move(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (targetPosition != transform.position) // Ensure there is a movement towards a target
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);
        }
    }

    private Vector3 FindNearestTarget()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        GameObject[] entityBases = GameObject.FindGameObjectsWithTag("EntityBase");
        Vector3 currentPosition = this.transform.position;

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
        {
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
        }

        return closestTarget != null ? closestTarget.transform.position : Vector3.positiveInfinity;
    }

    public void Taunt(Tank taunter)
    {
        tauntTarget = taunter.transform.position;
        isTaunted = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
        {
            NonEnemy entity = collision.gameObject.GetComponent<NonEnemy>();
            if (entity != null)
            {
                entity.TakeDamage(collisionDamage);
            }
         
        }
        
        if (collision.gameObject.CompareTag("EntityBase"))
        {
            EntityBases entitybase = collision.gameObject.GetComponent<EntityBases>();
            if (entitybase != null)
            {
                entitybase.TakeDamage(1000);
            }
            Destroy(gameObject);
        }
    }
}
