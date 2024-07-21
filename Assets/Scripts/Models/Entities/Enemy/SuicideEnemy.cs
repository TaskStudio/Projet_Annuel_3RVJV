using UnityEngine;

public class SuicideEnemy : Enemy
{
    protected override void AttackTarget()
    {
        Debug.Log("SuicideEnemy AttackTarget called");
        if (target != null)
        {
            Unit entity = target.GetComponent<Unit>();
            if (entity != null)
            {
                entity.TakeDamage(Data.attackDamage);
                Debug.Log("Damage dealt: " + Data.attackDamage + " to " + target.name);
                Debug.Log("Destroying SuicideEnemy: " + gameObject.name);
                Destroy(gameObject); // Ensure this is called
            }
            else
            {
                Debug.LogWarning("Target doesn't have a Unit component!");
            }
        }
        else
        {
            Debug.LogWarning("No target found for SuicideEnemy!");
        }
    }
}