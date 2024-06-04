using System.Collections.Generic;
using UnityEngine;

namespace Entities.Production
{
    public class EntityProductor : MonoBehaviour
    {
        [Space(10)] [Header("Production")]
        [SerializeField] private EntityDatabaseSO entityDatabase;
        [SerializeField] private EntityFactory entityFactory;
        [SerializeField] private Transform productionPoint;
 
        private Queue<Entity> productionQueue;

        private void Start()
        {
            productionQueue = new Queue<Entity>();
        }

        public void ProduceEntity(string entityID)
        {
            Entity entity = entityFactory.CreateEntity(entityID, productionPoint.position);
        }
    }
}