using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[Serializable]
public struct EntityAction
{
    public string actionName;
    public UnityEvent action;

    public int woodCost;
    public int stoneCost;
    public int goldCost;

    public EntityAction(string actionName, UnityAction action, int woodCost = 0, int stoneCost = 0, int goldCost = 0)
    {
        this.actionName = actionName;
        this.action = new UnityEvent();
        this.action.AddListener(action);

        this.woodCost = woodCost;
        this.stoneCost = stoneCost;
        this.goldCost = goldCost;
    }
}

public abstract class Entity : BaseObject
{
    protected static Dictionary<Collider, Entity> colliderToEntityMap = new();

    [Space(10)] [Header("ID")] [ShowOnly] [SerializeField]
    private string id;

    [SerializeField] private string addressableKey;

    [Space(10)] [Header("Display")] [SerializeField]
    private HealthBar healthBar;

    [Space(10)] [Header("Actions")] public List<EntityAction> actionList;

    [Space(10)] [Header("Placement")] [SerializeField]
    protected Material previewMaterial;

    [SerializeField] protected Material previewInvalidMaterial;
    [SerializeField] protected Material placedMaterial;

    [Space(5)] [SerializeField] protected MeshRenderer objectRenderer;

    public bool mapEditContext;
    private Collider entityCollider;

    public List<Unit> targetedBy { get; } = new();

    public int currentHealth { get; protected set; }
    public string ID { get; private set; }
    public string AddressableKey => addressableKey;

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
        UpdateHealthBar();
    }

    public int GetMaxHealthPoints()
    {
        return Data.maxHealthPoints;
    }

    public float GetMissingHealthPercentage()
    {
        return (float) currentHealth / Data.maxHealthPoints;
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

        UpdateHealthBar();
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
        UpdateHealthBar();
    }

    protected void SetMaxHealthPoints(int maxHealthPoints)
    {
        Data.maxHealthPoints = maxHealthPoints;
    }

    protected abstract void Die();

    internal void PreviewValid()
    {
        objectRenderer.materials = new[] { previewMaterial };
        objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
        objectRenderer.receiveShadows = false;
    }

    internal void PreviewInvalid()
    {
        objectRenderer.materials = new[] { previewInvalidMaterial };
        objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
        objectRenderer.receiveShadows = false;
    }

    internal void Place()
    {
        Initialize();
        objectRenderer.materials = new[] { placedMaterial };
        objectRenderer.shadowCastingMode = ShadowCastingMode.On;
        objectRenderer.receiveShadows = true;
    }

    protected void UpdateHealthBar()
    {
        healthBar.SetValue(currentHealth);
        healthBar.SetVisibility(currentHealth < Data.maxHealthPoints);
    }
}