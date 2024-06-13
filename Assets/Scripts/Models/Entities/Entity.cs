using Managers.Entities;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    public int hp = 100;
    public int currentHp;

    protected virtual void Start()
    {
        currentHp = hp; // Initialize currentHp to the maximum hp value
    }

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null) EntitiesManager.Instance.UnregisterMovableEntity(this as IMovable);

        if (SelectionManager.Instance != null) SelectionManager.Instance.DeselectEntity(this as ISelectable);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0) DestroyEntity();
        if (currentHp > hp) currentHp = hp; // Ensure currentHp does not exceed hp
    }

    public void Heal(int healAmount)
    {
        currentHp += healAmount;
        if (currentHp > hp) currentHp = hp; // Ensure currentHp does not exceed hp
    }

    protected void DestroyEntity()
    {
        Destroy(gameObject);
    }
}