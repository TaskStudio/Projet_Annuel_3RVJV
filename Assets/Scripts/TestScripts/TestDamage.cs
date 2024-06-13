using UnityEngine;

public class TestDamage : MonoBehaviour
{
    public BaseEntity entity; // Assign the entity in the inspector
    public int damageAmount = 10; // Amount of damage to apply

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDamage();
        }
    }

    private void ApplyDamage()
    {
        if (entity != null)
        {
            entity.TakeDamage(damageAmount);
            Debug.Log($"{entity.GetProfile().Name} took {damageAmount} damage. Current Health: {entity.Health}");
        }
        else
        {
            Debug.LogWarning("No entity assigned to TestDamage script.");
        }
    }
}