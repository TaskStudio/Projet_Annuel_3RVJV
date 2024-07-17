using UnityEngine;

public class Support : AllyFighter
{
    public float healRadius = 10f;
    public int healAmount = 10;
    public float healCooldown = 5f;
    private float healTimer;

    protected new void Start()
    {
        base.Start();
        healTimer = healCooldown;
    }

    protected new void Update()
    {
        base.Update();

        healTimer -= Time.deltaTime;
        if (healTimer <= 0f)
        {
            HealNearbyEntities();
            healTimer = healCooldown;
        }
    }

    private void HealNearbyEntities()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, healRadius);
        foreach (var hitCollider in hitColliders)
        {
            Unit entity = hitCollider.GetComponent<Unit>();
            if (entity != null && !(entity is Enemy)) // Heal only non-enemy entities
            {
                int healableAmount = Mathf.Min(
                    healAmount,
                    entity.GetMaxHealthPoints() - entity.GetHealthPoints()
                ); // Ensure we don't exceed max HP
                // entity.Heal(healableAmount); // Use the Heal method to increase currentHp
                //Debug.Log($"Healed {entity.gameObject.name} by {healableAmount} HP");
            }
        }
    }
}