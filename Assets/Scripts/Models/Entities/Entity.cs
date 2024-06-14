using UnityEngine;

public abstract class Entity : BaseEntity, IDamageable
{
    [SerializeField] protected int hp;
    public int MaxValue => hp;

    public int CurrentValue { get; set; }

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null) EntitiesManager.Instance.UnregisterMovableEntity(this as IMovable);

        if (SelectionManager.Instance != null) SelectionManager.Instance.DeselectEntity(this);
    }

    public int Health => hp;

    public void TakeDamage(int damage)
    {
        CurrentValue -= damage;
        if (CurrentValue < 0) CurrentValue = 0;
    }

    protected void DestroyEntity()
    {
        Destroy(gameObject);
    }
}