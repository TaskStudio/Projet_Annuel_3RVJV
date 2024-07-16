using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    private static UnitFactory instance;
    public static UnitFactory Instance;

    private static readonly Dictionary<string, Queue<IUnit>> unitPools = new();

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

    private void Start()
    {
        unitPools.Clear();

        foreach (var unit in GameManager.Instance.playerUnitDatabase.units)
        {
            unitPools.Add(unit.ID, new Queue<IUnit>());
            for (int i = 0; i < unit.PoolingAmount; i++)
            {
                IUnit unitInstance = Instantiate(unit.Prefab as Unit, Vector3.zero, Quaternion.identity);
                unitInstance.SetID(unit.ID);
                unitInstance.gameObject.SetActive(false);
                unitPools[unit.ID].Enqueue(unitInstance);
            }
        }
    }

    public static IUnit SpawnEntity(string unitID, Vector3 position, UnitDatabaseSO unitDatabase)
    {
        if (!unitPools.ContainsKey(unitID)) unitPools.Add(unitID, new Queue<IUnit>());

        IUnit unit;
        if (unitPools[unitID].Count > 0)
        {
            unit = unitPools[unitID].Dequeue();
            unit.transform.position = position;
            unit.gameObject.SetActive(true);
        }
        else
        {
            unit = Instantiate(unitDatabase.GetUnitPrefab(unitID), position, Quaternion.identity);
            unit.SetID(unitID);
        }

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
        if (!unitPools.ContainsKey(unit.ID)) unitPools.Add(unit.ID, new Queue<IUnit>());
        unitPools[unit.ID].Enqueue(unit);
    }
}