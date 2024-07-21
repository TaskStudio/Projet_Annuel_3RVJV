public abstract class Ally : Fighter
{


    protected override void Die()
    {
        base.Die();
        StatManager.IncrementAllyDeathCount();
    }
}