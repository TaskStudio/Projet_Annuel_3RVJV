using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEntity : IBaseObject
{
    string ID { get; }
    List<IEntity> targetedBy { get; }

    void TakeDamage(int damage);
    void SetHealthPoints(int currentHealthPoints);
    int GetHealthPoints();
    int GetMaxHealthPoints();
    float GetMissingHealthPercentage();
    bool IsDead();
    void SetID(string unitId);
    void AddTargetedBy(IEntity entity);
    void RemoveTargetedBy(IEntity entity);
    void TargetIsDead(IEntity entity);
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

    private void OnDisable()
    {
        SignalDeath();
    }

    public string ID { get; private set; }
    public List<IEntity> targetedBy { get; } = new();

    public int GetHealthPoints()
    {
        return currentHealth;
    }

    public void SetHealthPoints(int currentHealthPoints)
    {
        if (currentHealthPoints > data.maxHealthPoints) currentHealth = data.maxHealthPoints;
        else currentHealth = currentHealthPoints;

        healthBar.SetValue(currentHealth);
    }

    public int GetMaxHealthPoints()
    {
        return data.maxHealthPoints;
    }

    public float GetMissingHealthPercentage()
    {
        return (float) currentHealth / data.maxHealthPoints;
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

    public void AddTargetedBy(IEntity entity)
    {
        if (!targetedBy.Contains(entity)) targetedBy.Add(entity);
    }

    public void RemoveTargetedBy(IEntity entity)
    {
        if (targetedBy.Contains(entity)) targetedBy.Remove(entity);
    }

    public abstract void TargetIsDead(IEntity entity);

    public virtual void SignalDeath()
    {
        foreach (IEntity entity in targetedBy)
            if (entity != null)
            {
                entity.RemoveTargetedBy(this);
                entity.TargetIsDead(this);
            }

        SelectionManager.Instance.DeselectEntity(this);
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