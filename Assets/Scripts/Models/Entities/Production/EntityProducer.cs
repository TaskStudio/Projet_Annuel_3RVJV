using System.Collections.Generic;
using Entities;
using UnityEngine;

public class EntityProducer : Building
{
    [Space(10)] [Header("Production")]
    [SerializeField] private EntityDatabaseSO entityDatabase;
    [SerializeField] private Transform productionPoint;
    private readonly EntityFactory entityFactory = EntityFactory.Instance;

    private readonly Queue<string> productionQueue = new();
    private ResourceManager resourceManager;
    public float currentProductionTime { get; private set; }

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
    }

    private void Update()
    {
        if (productionQueue.Count <= 0) return;

        currentProductionTime -= Time.deltaTime;

        if (currentProductionTime > 0) return;

        string entityID = productionQueue.Dequeue();
        ProduceEntity(entityID);
    }

    public void RequestProduction(string entityID)
    {
        EntityData entityData = entityDatabase.GetEntityData(entityID);
        if (resourceManager.RequestResource(new Resource(entityData.ResourceType, entityData.Cost)))
            AddToProductionQueue(entityID);
    }

    public void AddToProductionQueue(string entityID)
    {
        productionQueue.Enqueue(entityID);
        if (productionQueue.Count == 1)
            currentProductionTime = entityDatabase.GetEntityData(entityID).ProductionTime;
    }

    private void ProduceEntity(string entityID)
    {
        entityFactory.SpawnEntity(entityID, productionPoint.position, entityDatabase);
        if (productionQueue.Count > 0)
            currentProductionTime = entityDatabase.GetEntityData(productionQueue.Peek()).ProductionTime;
    }
}