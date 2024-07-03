using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : Entity<EntityData>
{
}

public abstract class Entity<TDataType> : BaseObject<TDataType> where TDataType : EntityData
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;

    public int currentHealth { get; protected set; }
    public string faction { get; private set; }

    protected override void Initialize()
    {
        currentHealth = data.maxHealthPoints;
        faction = data.faction;
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
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}