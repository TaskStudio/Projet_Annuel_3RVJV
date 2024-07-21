using UnityEngine;

public class AllyFighter : Ally
{
    [SerializeField] private ParticleSystem attackParticleSystem;
    protected new void Update()
    {
        base.Update();

        if (Data.attackDamage != 0
            && (moveAttack || reachedDestination)
            && (targetsInRange.Count > 0 || currentTarget != null)) Attack();
    }

    public override void SetTarget(Entity target)
    {
        if (target == null) return;
        if (target is Enemy) currentTarget = target;
    }

    protected override void Attack()
    {
        base.Attack();
        if (Data.distanceType == DistanceType.Ranged && attackParticleSystem != null)
        {
            // Instantiate the particle system at the current position
        
            attackParticleSystem.transform.LookAt(currentTarget.transform); // Ensure the particles are directed towards the target
            attackParticleSystem.Play();
        }
        
    }
}