using System.Linq;
using UnityEngine;

public class AllyFighter : Fighter, IAlly
{
    protected new void Update()
    {
        base.Update();

        targetsInRange = Physics.OverlapSphere(transform.position, data.detectionRange)
            .Select(c => c.GetComponent<IEntity>())
            .Where(e => e is IEnemy)
            .ToList();

        if (data.attackDamage != 0
            && (moveAttack || reachedDestination)
            && (targetsInRange.Count > 0 || currentTarget != null)) Attack();
    }

    public override void SetTarget(IBaseObject target)
    {
        if (target == null) return;
        if (target is IEnemy) currentTarget = target as IEntity;
    }
}