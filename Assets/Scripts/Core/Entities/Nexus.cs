using UnityEngine;

public class Nexus : Entity
{
    public override void TargetIsDead(Entity entity)
    {
    }

    protected override void Die()
    {
        Debug.Log("Nexus is dead");
    }
}