public class AllyFighter : Fighter, IAlly
{
    public override void SetTarget(IBaseObject target)
    {
        if (target == null) return;
        if (target is IEnemy) currentTarget = target as IEntity;
    }

    public override void AddTargetInRange(IEntity target)
    {
        if (target is IEnemy)
        {
            targetsInRange.Add(target);
            target.AddTargetedBy(this);
        }
    }

    public override void RemoveTargetInRange(IEntity target)
    {
        if (target is IEnemy)
        {
            targetsInRange.Remove(target);
            target.RemoveTargetedBy(this);
        }
    }
}