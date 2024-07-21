using System.Collections;
using UnityEngine;

public class EnemyBoss : Enemy
{
    private float lastMeteorAttackTime;

    protected override void Start()
    {
        base.Start();
        lastMeteorAttackTime = -Data.attackCooldown; // Initial cooldown reset
    }

    protected override void AttackTarget()
    {
        if (Time.time >= lastMeteorAttackTime + Data.attackCooldown)
        {
            MeteorRainAttack();
            lastMeteorAttackTime = Time.time;
        }
        else
        {
            Attack();
            currentState = State.Idle;
        }
    }

    private void MeteorRainAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Data.attackRange, LayerMask.GetMask("Ally"));
        foreach (Collider hitCollider in hitColliders)
        {
            var entity = hitCollider.GetComponent<Entity>();
            if (entity != null)
            {
                entity.TakeDamage(Data.attackDamage);
            }
        }

        Debug.Log("Meteor rain attack unleashed!");
    }

    // Visualize the damage zone
    private void OnDrawGizmosSelected()
    {
        if (Data != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Data.attackRange);
        }
    }
}