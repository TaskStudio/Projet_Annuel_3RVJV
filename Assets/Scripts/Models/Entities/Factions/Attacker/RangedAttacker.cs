using UnityEngine;

public class RangedAttacker : Attacker
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float shootRange = 20f;
    public LayerMask enemyLayer;
    public float shootCooldown = 0.5f; // Cooldown time in seconds

    private float lastShootTime; // Time when the last shot was fired

    protected new void Start()
    {
        base.Start();
        lastShootTime = -shootCooldown; // Ensure the attacker can shoot immediately at start
    }

    protected new void Update()
    {
        base.Update();
        if (Time.time >= lastShootTime + shootCooldown)
        {
            Attack();
            lastShootTime = Time.time; // Update the last shoot time
        }
    }

    public override void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootRange, enemyLayer);
        if (hitColliders.Length > 0)
        {
            Vector3 enemyPosition = hitColliders[0].transform.position; // Attack the first enemy found
            Shoot(enemyPosition);
        }
    }

    private void Shoot(Vector3 target)
    {
        if (projectilePrefab && projectileSpawnPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            Vector3 shootDirection = (target - projectileSpawnPoint.position).normalized;

            // Initialize the projectile with direction, speed, and range
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
                projectileScript.Initialize(shootDirection, 20f, shootRange); // Set speed and range as necessary
        }
    }
}