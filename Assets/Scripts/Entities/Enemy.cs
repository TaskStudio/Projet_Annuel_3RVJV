using UnityEngine;

public class Enemy : Entity, IMovable
{
    public float moveSpeed = 5f;

    void Update()
    {
        Vector3 moveTarget = FindNearestTarget(); 
        Move(moveTarget);
    }

    public void Move(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);
        }
    }

    private Vector3 FindNearestTarget()
    {
        GameObject[] nonEnemies = GameObject.FindGameObjectsWithTag("NonEnemy"); 
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base"); 

        Vector3 closestTarget = Vector3.positiveInfinity;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Check non-enemies first
        foreach (GameObject target in nonEnemies)
        {
            float distance = (target.transform.position - currentPosition).sqrMagnitude;
            if (distance < closestDistanceSqr)
            {
                closestDistanceSqr = distance;
                closestTarget = target.transform.position;
            }
        }

        // Check bases if no non-enemy is close enough
        if (closestTarget == Vector3.positiveInfinity)
        {
            foreach (GameObject baseObj in bases)
            {
                float distance = (baseObj.transform.position - currentPosition).sqrMagnitude;
                if (distance < closestDistanceSqr)
                {
                    closestDistanceSqr = distance;
                    closestTarget = baseObj.transform.position;
                }
            }
        }

        return closestTarget;
    }
}
