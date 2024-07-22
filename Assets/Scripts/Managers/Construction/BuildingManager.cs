using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    public List<Building> BuildingsList { get; } = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddBuilding(Building building)
    {
        BuildingsList.Add(building);
    }

    public void RemoveBuilding(Building building)
    {
        BuildingsList.Remove(building);
    }

    public List<Building> GetBuildingsByState(Building.BuildingStates state)
    {
        return BuildingsList.FindAll(building => building.state == state);
    }

    public class buildingsDataJSON
    {
        public string name;
        public Vector3 position;
        public string role = "building";
        public Quaternion rotation;
        public string type;
    }
}