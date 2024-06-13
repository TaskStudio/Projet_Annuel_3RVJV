using System.Collections.Generic;
using Construction;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    public static BuildingsManager Instance;
    private static BuildingsManager instance;

    private readonly List<Building> buildings = new();
    private ResourceManager resourceManager;

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
        resourceManager = ResourceManager.Instance;
    }

    public void RegisterBuilding(Building building)
    {
        buildings.Add(building);

        if (building.behavior is ResourceStorage resourceStorage) resourceManager.RegisterStorage(resourceStorage);
    }
}