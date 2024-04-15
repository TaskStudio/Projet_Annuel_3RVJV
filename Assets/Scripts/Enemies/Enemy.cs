using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;
    public int damage = 1000;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
        {
            Entity entity = collision.gameObject.GetComponent<Entity>();
            if (entity != null)
            {
                entity.TakeDamage(100);
            }
            
            Destroy(this.gameObject);
        }
        
        if (collision.gameObject.CompareTag("EntityBase"))
        {
            EntityBase entitybase = collision.gameObject.GetComponent<EntityBase>();
            if (entitybase != null)
            {
                entitybase.TakeDamage(1000);
            }
            Destroy(gameObject);
        }
    }
    
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}