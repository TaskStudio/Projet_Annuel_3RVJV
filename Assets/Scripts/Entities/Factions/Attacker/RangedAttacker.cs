using UnityEngine;

public class RangedAttacker : Attacker
{
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        // Initialize Ranged specific properties if needed
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
        // Handle Ranged specific update logic if needed
    }

    public override void Shoot(Vector3 target)
    {
        base.Shoot(target);
    }

    public override void Attack()
    {
        // Implement the ranged attack logic here
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Shoot(mousePosition);
    }
}