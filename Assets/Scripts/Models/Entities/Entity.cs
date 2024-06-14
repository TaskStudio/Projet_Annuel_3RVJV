using UnityEngine;

public abstract class Entity : BaseEntity
{
    public int currentHp;
    [SerializeField] protected int hp;
    public int Health => hp;
    public int MaxValue => hp;
    public int CurrentValue { get; set; }

    protected virtual void Start()
    {
        currentHp = hp; // Initialize currentHp to the maximum hp value
    }

    protected virtual void OnDestroy()
    {
        if (EntitiesManager.Instance != null) EntitiesManager.Instance.UnregisterMovableEntity(this as IMovable);

        if (SelectionManager.Instance != null) SelectionManager.Instance.DeselectEntity(this);
    }


    public void TakeDamage(int damage)
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