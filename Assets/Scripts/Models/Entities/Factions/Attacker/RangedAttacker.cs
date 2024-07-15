using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttacker : Attacker
{
    public ParticleSystem attackParticles; // Assigned via the Unity Editor
    public List<ParticleCollisionEvent> collisionEvents;
    public float shootCooldown = 3f; // Cooldown time in seconds
    public int damageAmount = 100; // Set this to your desired damage value
    private bool canShoot = true;
    private float lastShootTime; // Time when the last shot was fired

    protected new void Start()
    {
        base.Start();
        lastShootTime = -shootCooldown;
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void Update()
    {
        base.Update();
        if (canShoot && attackParticles)
        {
            GameObject target = FindNearestEnemy(); // Find the nearest enemy
            if (target != null)
            {
                Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
                attackParticles.transform.rotation = lookRotation; // Aim the particle system at the target

                attackParticles.Play();
                StartCoroutine(ShootCooldown());
            }
        }
    }
    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject enemy in enemies)
        {
            Vector3 directionToTarget = enemy.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
    void OnParticleCollision(GameObject other)
    {

        
        int numCollisionEvents = attackParticles.GetCollisionEvents(other, collisionEvents);

        // Debugging: Check for Enemy component before entering if statement
        bool hasEnemyComponent = other.TryGetComponent<AttackingEnemy>(out AttackingEnemy enemy);
        Debug.Log($"Has Enemy Component: {hasEnemyComponent}");

        if (hasEnemyComponent)
        {
            Debug.Log($"Applying damage to: {enemy.name}, Damage: {damageAmount}"); // Confirm damage application
            enemy.TakeDamage(damageAmount); // Apply damage
            lastShootTime = Time.time; // Update last shoot time
        }
        else
        {
            Debug.LogError("Enemy component not found on collided object.");
        }
    }
}