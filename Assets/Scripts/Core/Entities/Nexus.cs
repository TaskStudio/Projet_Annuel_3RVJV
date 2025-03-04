using UnityEngine;

public class Nexus : UnitProducerBuilding
{
    [SerializeField] private int healthRegenPerSecond;

    private float timeSinceLastRegen;

    protected override void Update()
    {
        base.Update();
        RegenerateHealth();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        timeSinceLastRegen = 0f; // Reset time since last regeneration
    }

    protected override void Die()
    {
        GameManager.Instance.OnNexusDestroyed(this);
    }

    private void RegenerateHealth()
    {
        if (healthRegenPerSecond > 0 && currentHealth < Data.maxHealthPoints)
        {
            timeSinceLastRegen += Time.deltaTime;

            if (timeSinceLastRegen >= 1f)
            {
                var healthToRegen = Mathf.RoundToInt(healthRegenPerSecond * timeSinceLastRegen);
                currentHealth = Mathf.Min(currentHealth + healthToRegen, Data.maxHealthPoints);
                UpdateHealthBar();
                timeSinceLastRegen = 0f;
            }
        }
    }
}