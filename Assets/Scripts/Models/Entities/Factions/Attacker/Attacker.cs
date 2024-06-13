using UnityEngine;

public abstract class Attacker : NonEnemy
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