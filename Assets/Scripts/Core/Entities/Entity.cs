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
    [SerializeField] private string addressableKey;
    protected static Dictionary<Collider, Entity> colliderToEntityMap = new();

    [Space(10)] [Header("ID")] [ShowOnly] [SerializeField]
    private string id;

    [Space(10)] [Header("Display")] [SerializeField]
    private HealthBar healthBar;

    public string ID { get; private set; }
    public string AddressableKey => addressableKey;
    [Space(10)] [Header("Actions")] public List<EntityAction> actionList;

    private Collider entityCollider;
    public List<Unit> targetedBy { get; } = new();

    public int currentHealth { get; protected set; }

    public string ID { get; private set; }

    private void Awake()
    {
        entityCollider = GetComponent<Collider>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        colliderToEntityMap[entityCollider] = this;
    }

    private void OnDisable()
    {
        colliderToEntityMap.Remove(entityCollider);
        SignalDeath();
    }

    public void AddTargetedBy(Unit unit)
    {
        if (!targetedBy.Contains(unit)) targetedBy.Add(unit);
    }

    public void RemoveTargetedBy(Unit unit)
    {
        if (targetedBy.Contains(unit)) targetedBy.Remove(unit);
    }

    public void SignalDeath()
    {
        foreach (var unit in targetedBy)
            if (unit != null)
                unit.TargetIsDead(this);

        SelectionManager.Instance.DeselectEntity(this);
    }

    public int GetHealthPoints()
    {
        return currentHealth;
    }

    public void SetHealthPoints(int currentHealthPoints)
    {
        if (currentHealthPoints > Data.maxHealthPoints) currentHealth = Data.maxHealthPoints;
        else currentHealth = currentHealthPoints;

        healthBar.SetValue(currentHealth);
        healthBar.SetVisibility(currentHealth < Data.maxHealthPoints);
    }

    public int GetMaxHealthPoints()
    {
        return Data.maxHealthPoints;
    }

    public float GetMissingHealthPercentage()
    {
        return (float)currentHealth / Data.maxHealthPoints;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        healthBar.SetValue(currentHealth);
        healthBar.SetVisibility(currentHealth < Data.maxHealthPoints);
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
        healthBar.SetVisibility(false); // Ensure health bar is initially hidden
    }

    protected void SetMaxHealthPoints(int maxHealthPoints)
    {
        Data.maxHealthPoints = maxHealthPoints;
    }

    protected abstract void Die();
}