using UnityEngine;

public abstract class Attacker : NonEnemy
{
    // Additional properties or methods specific to all attackers can be added here

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        // Initialize common Attacker properties if needed
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
        // Handle common Attacker update logic if needed
    }

    public abstract void Attack();  // Each specific type of Attacker will implement this
}