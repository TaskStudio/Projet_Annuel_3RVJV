using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu]
    public class EntityDatabaseSO : ScriptableObject
    {
        public List<EntityData> entitiesData;

        public Entity GetEntityPrefab(string entityID)
        {
            EntityData entityData = entitiesData.Find(data => data.ID == entityID);
            if (entityData == null)
            {
                Debug.LogError("Entity with ID " + entityID + " not found in database.");
                return null;
            }

            return entityData.Prefab;
        }

        public EntityData GetEntityData(string entityID)
        {
            return entitiesData.Find(data => data.ID == entityID);
        }
    }

    [Serializable]
    public class EntityData
    {
        [field: SerializeField] public int IdNumber { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Entity Prefab { get; private set; }
        [field: SerializeField] public float ProductionTime { get; private set; }

        public EntityData()
        {
            ID = "ENT-" + IdNumber;
        }

        public string ID { get; private set; }
    }
}