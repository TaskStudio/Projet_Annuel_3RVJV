using UnityEngine;

public class SuicideEnemy : Enemy
{
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
        {
            Unit entity = collision.gameObject.GetComponent<Unit>();
            if (entity != null)
            {
                entity.TakeDamage(1000);
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("EntityBase"))
        {
            EntityBases entityBase = collision.gameObject.GetComponent<EntityBases>();
            if (entityBase != null) entityBase.TakeDamage(1000);
            Destroy(gameObject);
        }
    }

    protected override void AttackTarget()
    {
        if (target != null)
        {
            Unit entity = target.GetComponent<Unit>();
            if (entity != null) entity.TakeDamage(1000);
            Destroy(gameObject);
        }
    }
}