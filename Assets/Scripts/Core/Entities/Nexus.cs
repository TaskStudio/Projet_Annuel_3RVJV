using UnityEngine;

public class Nexus : Entity
{
    private void Update()
    {
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space)) TakeDamage(100);
    }

    protected override void Die()
    {
        Debug.Log("Nexus is dead");
        GameManager.Instance.OnNexusDestroyed(this);
    }
}