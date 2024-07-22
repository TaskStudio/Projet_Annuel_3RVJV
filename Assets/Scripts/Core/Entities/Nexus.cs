using UnityEngine;

public class Nexus : UnitProducerBuilding
{
    [SerializeField] private int healthRegenPerSecond;

    private float timeSinceLastRegen;

    private new void Update()
    {
        base.Update();
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space)) TakeDamage(100);
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
        Debug.Log("healt : " + GetMaxHealthPoints());
        Debug.Log("regen : " + healthRegenPerSecond);

        if (healthRegenPerSecond > 0 && currentHealth < Data.maxHealthPoints)
        {
            Debug.Log("HELLO");

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