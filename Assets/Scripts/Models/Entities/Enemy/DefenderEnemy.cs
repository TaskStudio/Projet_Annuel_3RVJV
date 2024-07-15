using UnityEngine;

public class DefenderEnemy : Enemy
{
    public int extraHealth = 100;

    protected override void Start()
    {
        base.Start();
        currentHealth += extraHealth; 
    }

    protected override void AttackTarget()
    {
        if (target != null)
        {
            Unit entity = target.GetComponent<Unit>();
            if (entity != null)
            {
                entity.TakeDamage(10); 
            }
            currentState = State.Idle;
        }
    }
}