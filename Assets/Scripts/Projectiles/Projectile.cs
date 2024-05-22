using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 100;
    private Vector3 velocity; // to store the direction and speed

    public void Initialize(Vector3 direction, float speed)
    {
        this.velocity = direction * speed;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
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
        
        if (collision.gameObject.CompareTag("EnemyBase"))
        {
            EnemyBase enemybase = collision.gameObject.GetComponent<EnemyBase>();
            if (enemybase != null)
            {
                enemybase.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}