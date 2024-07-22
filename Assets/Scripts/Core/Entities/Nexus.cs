using UnityEngine;

public class Nexus : UnitProducerBuilding
{
    // private new void Update()
    // {
    //     base.Update();
    //     // Check if the spacebar is pressed
    //     if (Input.GetKeyDown(KeyCode.Space)) TakeDamage(100);
    // }

    protected override void Die()
    {
        Debug.Log("Nexus is dead");
        GameManager.Instance.OnNexusDestroyed(this);
    }
}