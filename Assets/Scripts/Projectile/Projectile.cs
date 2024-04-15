using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 100;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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