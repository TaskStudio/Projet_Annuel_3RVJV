using System.Collections.Generic;
using UnityEngine;

public class UnitProducerBuilding : Building
{
    [Space(10)] [Header("Production")]
    [SerializeField] private UnitDatabaseSO unitDatabase;
    [SerializeField] private Transform productionPoint;
    private readonly UnitFactory entityFactory;

    private readonly Queue<string> productionQueue = new();
    private float currentProductionTime { get; set; }

    private new void Update()
    {
        base.Update();

        if (productionQueue.Count <= 0) return;

        currentProductionTime -= Time.deltaTime;

        if (currentProductionTime > 0) return;

        string entityID = productionQueue.Dequeue();
        ProduceEntity(entityID);
    }

    public void RequestProduction(string entityID)
    {
        UnitDatabaseData entityData = unitDatabase.GetEntityData(entityID);
        if (ResourceManager.Instance.RequestResource(new Resource(entityData.ResourceType, entityData.Cost)))
            AddToProductionQueue(entityID);
    }

    public void AddToProductionQueue(string entityID)
    {
        productionQueue.Enqueue(entityID);
        if (productionQueue.Count == 1)
            currentProductionTime = unitDatabase.GetEntityData(entityID).ProductionTime;
    }

    private void ProduceEntity(string entityID)
    {
        UnitFactory.SpawnEntity(entityID, productionPoint.position, unitDatabase);
        if (productionQueue.Count > 0)
            currentProductionTime = unitDatabase.GetEntityData(productionQueue.Peek()).ProductionTime;
    }
}