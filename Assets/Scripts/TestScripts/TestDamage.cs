using UnityEngine;

public class TestDamage : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage to apply
    public GameObject entity; // Assign the entity in the inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ApplyDamage();
    }

    private void ApplyDamage()
    {
        if (entity != null)
            entity.GetComponent<IDamageable>()?.TakeDamage(damageAmount);
        else
            Debug.LogWarning("No entity assigned to TestDamage script.");
    }
}