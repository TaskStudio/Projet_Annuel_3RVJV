using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitDatabaseSO : ScriptableObject
{
    public List<UnitDatabaseData> units;

    public Unit GetUnitPrefab(string unitID)
    {
        UnitDatabaseData unitDatabaseData = units.Find(data => data.ID == unitID);
        if (unitDatabaseData == null)
        {
            Debug.LogError("Entity with ID " + unitID + " not found in database.");
            return null;
        }

        return unitDatabaseData.Prefab as Unit;
    }

    public UnitDatabaseData GetEntityData(string entityID)
    {
        return units.Find(data => data.ID == entityID);
    }
}

[Serializable]
public class UnitDatabaseData
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public BaseObject Prefab { get; private set; }
    [field: SerializeField] public float ProductionTime { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public Resource.Type ResourceType { get; private set; }
    [field: SerializeField] public int PoolingAmount { get; private set; } = 20;
}