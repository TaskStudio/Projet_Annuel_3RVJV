using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu]
    public class EntityDatabaseSO : ScriptableObject
    {
        public List<EntityData> entitiesData;
    }

    [Serializable]
    public class EntityData
    {
        [field: SerializeField] public int IdNumber { get; private set; }

        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Entity Prefab { get; private set; }

        public EntityData()
        {
            ID = "ENT-" + IdNumber;
        }

        public string ID { get; private set; }
    }
}