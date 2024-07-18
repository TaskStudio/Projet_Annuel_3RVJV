using System.Linq;
using UnityEngine;

public class AllyFighter : Fighter, IAlly
{
    protected new void Update()
    {
        base.Update();

        var enemiesInRange = Physics.OverlapSphere(transform.position, Data.detectionRange)
            .Select(c => c.GetComponent<Entity>())
            .Where(e => e is Enemy)
            .ToList();

        targetsInRange = enemiesInRange.ToList();

        if (Data.attackDamage != 0
            && (moveAttack || reachedDestination)
            && (targetsInRange.Count > 0 || currentTarget != null)) Attack();
    }

    public override void SetTarget(Entity target)
    {
        if (target == null) return;
        if (target is IEnemy) currentTarget = target;
    }
}