using Managers.Entities;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    public int hp = 100;

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null) EntitiesManager.Instance.UnregisterMovableEntity(this as IMovable);

        if (SelectionManager.Instance != null) SelectionManager.Instance.DeselectEntity(this as ISelectable);
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) DestroyEntity();
    }

    protected void DestroyEntity()
    {
        Destroy(gameObject);
    }
}