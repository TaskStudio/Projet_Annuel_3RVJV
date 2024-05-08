using UnityEngine;

public class Projectilee : MonoBehaviour
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
            Enemyy enemy = collision.gameObject.GetComponent<Enemyy>();
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