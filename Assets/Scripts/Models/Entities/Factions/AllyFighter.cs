using UnityEngine;

public class AllyFighter : Ally
{
    
    protected new void Update()
    {
        base.Update();

        if (Data.attackDamage != 0
            && (moveAttack || reachedDestination)
            && (targetsInRange.Count > 0 || currentTarget != null)) Attack();
    }

    public override void SetTarget(Entity target)
    {
        if (target == null) return;
        if (target is Enemy) currentTarget = target;
    }
    
}