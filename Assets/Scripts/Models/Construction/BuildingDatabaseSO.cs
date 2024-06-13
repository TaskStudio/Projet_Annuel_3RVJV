using System;
using System.Collections.Generic;
using UnityEngine;

namespace Construction
{
    [CreateAssetMenu]
    public class BuildingDatabaseSO : ScriptableObject
    {
        public List<BuildingData> buildingsData;
    }

    [Serializable]
    public class BuildingData
    {
        [field: SerializeField] public int IdNumber { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
        [field: SerializeField] public Building Prefab { get; private set; }
        [field: SerializeField] public float ConstructionTime { get; private set; }

        public BuildingData()
        {
            ID = "BLD-" + IdNumber;
        }

        public string ID { get; private set; }
    }
}