using UnityEngine;

public class Nexus : Entity<EntityData>
{
    public override void TargetIsDead(IEntity entity)
    {
    }

    protected override void Die()
    {
        Debug.Log("Nexus is dead");
    }
}