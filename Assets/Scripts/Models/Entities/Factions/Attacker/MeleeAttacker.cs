using UnityEngine;

public class MeleeAttacker : Attacker
{
    private int meleeDamage = 40;

    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        base.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
    }

    public override void Attack()
    {
        // Implementation of other melee attack logic here
    }
}