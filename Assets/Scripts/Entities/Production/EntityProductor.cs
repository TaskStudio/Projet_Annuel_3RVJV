using System.Collections.Generic;
using UnityEngine;

namespace Entities.Production
{
    public class EntityProductor : MonoBehaviour
    {
        [Space(10)] [Header("Production")]
        [SerializeField] private EntityDatabaseSO entityDatabase;
        [SerializeField] private EntityFactoryManager entityFactoryManager;
        [SerializeField] private Transform productionPoint;

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

        private void AddToProductionQueue(string entityID)
        {
            productionQueue.Enqueue(entityID);
            if (productionQueue.Count == 1)
                currentProductionTime = entityDatabase.GetEntityData(entityID).ProductionTime;
        }

        public void ProduceEntity(string entityID)
        {
            Entity entity = entityFactoryManager.SpawnEntity(entityID, productionPoint.position, entityDatabase);
            if (productionQueue.Count > 0)
                currentProductionTime = entityDatabase.GetEntityData(productionQueue.Peek()).ProductionTime;
        }
    }
}