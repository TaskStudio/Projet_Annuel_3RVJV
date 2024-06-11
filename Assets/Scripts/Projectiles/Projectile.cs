using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 100;
    private Vector3 velocity; // to store the direction and speed
    private Vector3 startPosition; // to store the starting position
    private float maxRange; // the maximum range the projectile can travel

    public void Initialize(Vector3 direction, float speed, float range)
    {
        this.velocity = direction * speed;
        this.maxRange = range;
        this.startPosition = transform.position;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        
        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyBase"))
        {
            EnemyBase enemyBase = collision.gameObject.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}