using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitFactory : MonoBehaviour
{
    private static UnitFactory instance;
    public static UnitFactory Instance;

    private static readonly Dictionary<string, Queue<Unit>> unitPools = new();

    [FormerlySerializedAs("playerEntityDatabase")]
    public UnitDatabaseSO playerUnitDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        unitPools.Clear();

        foreach (var unit in playerUnitDatabase.units)
        {
            unitPools.Add(unit.ID, new Queue<Unit>());
            for (var i = 0; i < unit.PoolingAmount; i++)
            {
                var unitInstance = Instantiate(unit.Prefab, Vector3.zero, Quaternion.identity);
                if (unitInstance is Unit iunit)
                {
                    iunit.SetID(unit.ID);
                    unitInstance.gameObject.SetActive(false);
                    unitPools[unit.ID].Enqueue(iunit);
                }
            }
        }
    }

    public static Unit SpawnEntity(string unitID, Vector3 position, UnitDatabaseSO unitDatabase)
    {
        if (!unitPools.ContainsKey(unitID)) unitPools.Add(unitID, new Queue<Unit>());

        Unit unit;
        if (unitPools[unitID].Count > 0)
        {
            unit = unitPools[unitID].Dequeue();
            unit.transform.position = position;
            unit.gameObject.SetActive(true);
        }
        else
        {
            unit = Instantiate(unitDatabase.GetUnitPrefab(unitID), position, Quaternion.identity) as Unit;
            unit.SetID(unitID);
        }

        StatManager.IncrementUnitProductionCount();
        return unit;
    }

    public static void ReturnEntity(Unit unit)
    {
        if (unit.ID == null)
        {
            Destroy(unit.gameObject);
            return;
        }

        unit.gameObject.SetActive(false);
        if (!unitPools.ContainsKey(unit.ID)) unitPools.Add(unit.ID, new Queue<Unit>());
        unitPools[unit.ID].Enqueue(unit);
    }
}