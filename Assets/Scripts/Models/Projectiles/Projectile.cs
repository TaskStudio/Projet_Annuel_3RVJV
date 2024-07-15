using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    private float maxRange; // the maximum range the projectile can travel
    private Vector3 startPosition; // to store the starting position
    private Vector3 velocity; // to store the direction and speed

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyBase"))
        {
            EnemyBase enemyBase = collision.gameObject.GetComponent<EnemyBase>();
            if (enemyBase != null) enemyBase.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 direction, float speed, float range)
    {
        velocity = direction * speed;
        maxRange = range;
        startPosition = transform.position;
    }
}