public abstract class Entity : BaseEntity, IDamageable
{
    public int hp = 100;

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null) EntitiesManager.Instance.UnregisterMovableEntity(this as IMovable);

        if (SelectionManager.Instance != null) SelectionManager.Instance.DeselectEntity(this);
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