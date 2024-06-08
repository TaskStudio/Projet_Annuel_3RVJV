using UnityEngine;

public class MeleeAttacker : Attacker
{
    public int meleeDamage = 20;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        // Initialize Melee specific properties if needed
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
        // Handle Melee specific update logic if needed
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
        // Implement the melee attack logic here
        // In this case, melee attack is handled via OnCollisionEnter
    }
}