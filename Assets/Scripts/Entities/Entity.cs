using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    public int hp = 100;

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}