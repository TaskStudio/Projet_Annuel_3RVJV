public abstract class Attacker : Fighter
{
    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        base.Update();
    }

    public abstract void Attack();
}