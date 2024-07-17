using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : Entity<EntityData>
{
}

public abstract class Entity<TDataType> : BaseObject<TDataType> where TDataType : EntityData
{
    [Space(10)] [Header("ID")] [ShowOnly] [SerializeField]
    private string id;

    [Space(10)] [Header("Actions")] public List<UnityEvent> actionList;

    [Space(10)] [Header("Display")] [SerializeField]
    private HealthBar healthBar;

    public string ID { get; private set; }


    public int currentHealth { get; protected set; }

    public void SetID(string unitId)
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = unitId;
            id = unitId;
        }
    }

    protected override void Initialize()
    {
        SetHealthPoints(data.maxHealthPoints);
        healthBar.Initialize(data.maxHealthPoints);
    }

    public int GetHealthPoints()
    {
        return currentHealth;
    }

    public void SetHealthPoints(int currentHealthPoints)
    {
        currentHealth = currentHealthPoints;
    }

    public int GetMaxHealthPoints()
    {
        return data.maxHealthPoints;
    }

    protected void SetMaxHealthPoints(int maxHealthPoints)
    {
        data.maxHealthPoints = maxHealthPoints;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        healthBar.SetValue(currentHealth);
    }

    protected abstract void Die();
}