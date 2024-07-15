using UnityEngine;

public class AttackingEnemy : Enemy
{
    protected override void AttackTarget()
    {
        if (target != null)
        {
            Unit entity = target.GetComponent<Unit>();
            if (entity != null)
            {
                entity.TakeDamage(20); 
            }
            currentState = State.Idle;
        }
    }
}