using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
///     Class <c>Fighter</c> represents a Unit that can attack other Entities
/// </summary>
public class Fighter : Unit
{
    public enum Distance
    {
        Melee,
        Ranged
    }

    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private Distance distance;
    [SerializeField] [CanBeNull] private GameObject projectilePrefab;
    [SerializeField] [CanBeNull] private Transform projectileSpawnPoint;

    private float lastAttackTime;
    private Entity target;

    protected new void Start()
    {
        base.Start();
        lastAttackTime = -attackCooldown; // Ensure the attacker can shoot immediately at start
    }

    protected new void Update()
    {
        base.Update();
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time; // Update the last shoot time
        }
    }

    private void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        if (hitColliders.Length > 0)
        {
            Vector3? enemyPosition = Array.Find(hitColliders, c => c.CompareTag("Enemy") || c.CompareTag("EnemyBase"))
                ?.transform.position;
            if (enemyPosition.HasValue)
            {
                if (Vector3.Distance(transform.position, enemyPosition.Value) >= attackRange)
                {
                    targetPosition = enemyPosition.Value;
                    return;
                }

                targetPosition = transform.position;
                gameObject.transform.LookAt(enemyPosition.Value);
                if (distance == Distance.Ranged)
                {
                    GameObject projectile = Instantiate(
                        projectilePrefab,
                        projectileSpawnPoint.position,
                        Quaternion.identity
                    );
                    Vector3 shootDirection = (enemyPosition.Value - projectileSpawnPoint.position).normalized;
                    Projectile projectileScript = projectile.GetComponent<Projectile>();
                    if (projectileScript != null)
                        projectileScript.Initialize(
                            shootDirection,
                            20f,
                            detectionRange
                        );
                }
            }
        }
    }
}