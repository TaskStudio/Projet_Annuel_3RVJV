using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    public static BuildingsManager Instance;
    private static BuildingsManager instance;

    private readonly List<Building> buildings = new();

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

    public void RegisterBuilding(Building building)
    {
        buildings.Add(building);
    }
}