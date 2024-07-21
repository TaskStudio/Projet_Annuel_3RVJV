using UnityEngine;

public class SuicideEnemy : Enemy
{
    protected override void AttackTarget()
    {
        if (target != null)
        {
            Unit entity = target.GetComponent<Unit>();
            if (entity != null) entity.TakeDamage(Data.attackDamage);
            Debug.Log("Damage : " + Data.attackDamage );
            Destroy(gameObject);
        }
    }
}