using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct EntityAction
{
    public string actionName;
    public UnityEvent action;
}

public abstract class Entity : BaseObject
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
        if (currentHealthPoints > Data.maxHealthPoints) currentHealth = Data.maxHealthPoints;
        else currentHealth = currentHealthPoints;

        healthBar.SetValue(currentHealth);
    }

    public int GetMaxHealthPoints()
    {
        return Data.maxHealthPoints;
    }

    public float GetMissingHealthPercentage()
    {
        return (float) currentHealth / Data.maxHealthPoints;
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
        currentHealth = Data.maxHealthPoints;
        healthBar.Initialize(Data.maxHealthPoints);
    }

    protected void SetMaxHealthPoints(int maxHealthPoints)
    {
        Data.maxHealthPoints = maxHealthPoints;
    }

    protected abstract void Die();
}