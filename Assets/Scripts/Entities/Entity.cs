using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    public int hp = 100;

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, remaining HP: {hp}");
        if (hp <= 0)
        {
            DestroyEntity();
        }
    }

    protected void DestroyEntity()
    {
        Debug.Log($"{gameObject.name} is destroyed.");
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