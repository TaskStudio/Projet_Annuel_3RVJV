using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class EntityFactory : MonoBehaviour
    {
        private static EntityFactory instance;
        public static EntityFactory Instance;

        private readonly Dictionary<string, Queue<Entity>> entityPools = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public Entity SpawnEntity(string entityID, Vector3 position, EntityDatabaseSO entityDatabase)
        {
            if (!entityPools.ContainsKey(entityID)) entityPools.Add(entityID, new Queue<Entity>());

            Entity entity;
            if (entityPools[entityID].Count > 0)
            {
                entity = entityPools[entityID].Dequeue();
                entity.transform.position = position;
                entity.gameObject.SetActive(true);
            }
            else
            {
                entity = Instantiate(entityDatabase.GetEntityPrefab(entityID), position, Quaternion.identity);
            }

            return entity;
        }

        public void ReturnEntity(Entity entity, string entityID)
        {
            entity.gameObject.SetActive(false);
            if (!entityPools.ContainsKey(entityID)) entityPools.Add(entityID, new Queue<Entity>());
            entityPools[entityID].Enqueue(entity);
        }
    }
}