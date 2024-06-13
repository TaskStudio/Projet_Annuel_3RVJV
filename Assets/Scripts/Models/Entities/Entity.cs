using Managers.Entities;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    [SerializeField] protected int hp;

    public int Health => hp;
    public int MaxValue => hp;
    private int currentHealth;
    public int CurrentValue
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
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