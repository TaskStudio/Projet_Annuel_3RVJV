using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    public int hp = 100;

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;  
        if (Health <= 0)
        {
            DestroyEntity();  
        }
    }
    
    protected void DestroyEntity()
    {
        Destroy(gameObject);  
    }

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null)
        {
            EntitiesManager.Instance.UnregisterMovableEntity(this as IMovable);
        }
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.DeselectEntity(this as ISelectable);
        }
    }

}