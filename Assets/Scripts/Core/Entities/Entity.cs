using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : BaseObject
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;
    
    public EntityData entityData;
    public int currentHealth;
    public int currentMana = 0;
    public float attackSpeed = 1.0f;
    public float movementSpeed = 0.5f;
    public string race;

    private void Start()
    {
        Initialize(entityData);
    }
    
    public override void Initialize(ObjectData data)
    {
        base.Initialize(data);
        if (data is EntityData entityData)
        {
            this.entityData = entityData;
            currentHealth = entityData.maxHealthPoints;
            currentMana = entityData.maxManaPoints;
            attackSpeed = entityData.attackSpeed;
            movementSpeed = entityData.movementSpeed;
            race = entityData.race;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentHealth <= 0)
        {
            Die();
        }
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
        return entityData.maxHealthPoints;
    }

    public void SetMaxHealthPoints(int maxHealthPoints)
    {
        entityData.maxHealthPoints = maxHealthPoints;
    }

    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }
}