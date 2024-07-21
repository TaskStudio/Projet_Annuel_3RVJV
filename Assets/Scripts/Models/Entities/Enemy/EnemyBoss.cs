using System.Collections;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public GameObject meteorParticlePrefab;
    private float lastMeteorAttackTime;
    private GameObject meteorParticleInstance;
    private ParticleSystem meteorParticleSystem;
    
    public Vector3 particleOffset = new Vector3(0, 2.35f, 0);

    protected override void Start()
    {
        base.Start();
        lastMeteorAttackTime = -Data.attackCooldown;
        if (meteorParticlePrefab != null)
        {
            meteorParticleInstance = Instantiate(meteorParticlePrefab);
            meteorParticleInstance.SetActive(false);
            meteorParticleSystem = meteorParticleInstance.GetComponent<ParticleSystem>();
        }
    }

    private void LateUpdate()
    {
        if (meteorParticleInstance != null && meteorParticleInstance.activeSelf)
        {
            // Ensure the particle system follows the boss position with offset
            meteorParticleInstance.transform.position = transform.position + particleOffset;
        }
    }

    protected override void AttackTarget()
    {
        if (Time.time >= lastMeteorAttackTime + Data.attackCooldown)
        {
            StartCoroutine(MeteorRainAttack());
            lastMeteorAttackTime = Time.time;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private IEnumerator MeteorRainAttack()
    {
        if (meteorParticleInstance != null && meteorParticleSystem != null)
        {
            // Update and activate the particle system at the boss's position
            meteorParticleInstance.transform.position = transform.position + particleOffset;
            meteorParticleInstance.SetActive(true); 

            meteorParticleSystem.Play();
            // Deactivate it after the duration of the particle effect
            StartCoroutine(DeactivateParticleSystem(meteorParticleSystem, meteorParticleSystem.main.duration));
        }

        // Short delay to ensure particles are visible before damage is applied
        yield return new WaitForSeconds(0.5f);

        var hitColliders = Physics.OverlapSphere(transform.position, Data.attackRange, LayerMask.GetMask("Ally"));
        foreach (var hitCollider in hitColliders)
        {
            var entity = hitCollider.GetComponent<Entity>();
            if (entity != null) entity.TakeDamage(Data.attackDamage);
        }
    }

    private IEnumerator DeactivateParticleSystem(ParticleSystem particleSystem, float delay)
    {
        yield return new WaitForSeconds(delay);
        particleSystem.Stop();
        meteorParticleInstance.SetActive(false); 
    }
}
