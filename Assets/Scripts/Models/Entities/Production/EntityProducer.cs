using System.Collections.Generic;
using UnityEngine;

namespace Entities.Production
{
    public class EntityProducer : Actionable
    {
        [Space(10)] [Header("Production")]
        [SerializeField] private EntityDatabaseSO entityDatabase;
        [SerializeField] private Transform productionPoint;
        private readonly EntityFactory entityFactory = EntityFactory.Instance;

        private Queue<string> productionQueue;
        public float currentProductionTime { get; private set; }

        private void Start()
        {
            productionQueue = new Queue<string>();
        }

        private void Update()
        {
            if (productionQueue.Count <= 0) return;

            currentProductionTime -= Time.deltaTime;

            if (currentProductionTime > 0) return;

            string entityID = productionQueue.Dequeue();
            ProduceEntity(entityID);
        }

        public void AddToProductionQueue(string entityID)
        {
            productionQueue.Enqueue(entityID);
            if (productionQueue.Count == 1)
                currentProductionTime = entityDatabase.GetEntityData(entityID).ProductionTime;
        }

        private void ProduceEntity(string entityID)
        {
            BaseEntity entity = entityFactory.SpawnEntity(entityID, productionPoint.position, entityDatabase);
            if (productionQueue.Count > 0)
                currentProductionTime = entityDatabase.GetEntityData(productionQueue.Peek()).ProductionTime;
        }
    }
}