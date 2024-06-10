using UnityEngine;

public class MeleeAttacker : Attacker
{
    public int meleeDamage = 40;

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
                Debug.Log($"{enemy.name} took {meleeDamage} damage, remaining HP: {enemy.hp}");
            }
        }
    }

    public override void Attack()
    {
        // Implement the melee attack logic here
        // In this case, melee attack is handled via OnCollisionEnter
    }
}