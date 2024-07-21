public abstract class Ally : Fighter
{


    protected override void Die()
    {
        base.Die();
        StatManager.IncrementAllyDeathCount();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        StatManager.IncrementAllyDamageTaken(damage);
    }
}