using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class EntityFactory : MonoBehaviour
    {
        private static EntityFactory instance;
        public static EntityFactory Instance;

        private readonly Dictionary<string, Queue<BaseEntity>> entityPools = new();

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

        public BaseEntity SpawnEntity(string entityID, Vector3 position, EntityDatabaseSO entityDatabase)
        {
            if (!entityPools.ContainsKey(entityID)) entityPools.Add(entityID, new Queue<BaseEntity>());

            BaseEntity entity;
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

        public void ReturnEntity(BaseEntity entity, string entityID)
        {
            entity.gameObject.SetActive(false);
            if (!entityPools.ContainsKey(entityID)) entityPools.Add(entityID, new Queue<BaseEntity>());
            entityPools[entityID].Enqueue(entity);
        }
    }
}