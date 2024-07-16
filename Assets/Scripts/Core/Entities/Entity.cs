using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEntity : IBaseObject
{
    string ID { get; }

    void TakeDamage(int damage);
    void SetHealthPoints(int currentHealthPoints);
    int GetHealthPoints();
    int GetMaxHealthPoints();
    bool IsDead();
    void SetID(string unitId);
}

[Serializable]
public struct EntityAction
{
    public string actionName;
    public UnityEvent action;
}

public abstract class Entity : Entity<EntityData>
{
}

public abstract class Entity<TDataType> : BaseObject<TDataType>, IEntity where TDataType : EntityData
{
    [Space(10)] [Header("ID")]
    [ShowOnly] [SerializeField] private string id;

    [Space(10)] [Header("Display")]
    [SerializeField] private HealthBar healthBar;

    [Space(10)] [Header("Actions")]
    public List<EntityAction> actionList;


    public int currentHealth { get; protected set; }

    public string ID { get; private set; }

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

    public void SetID(string unitId)
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = unitId;
            id = unitId;
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    protected override void Initialize()
    {
        currentHealth = data.MaxHealthPoints;
        healthBar.Initialize(data.MaxHealthPoints);
    }

    protected void SetMaxHealthPoints(int maxHealthPoints)
    {
        data.maxHealthPoints = maxHealthPoints;
    }

    protected abstract void Die();
}