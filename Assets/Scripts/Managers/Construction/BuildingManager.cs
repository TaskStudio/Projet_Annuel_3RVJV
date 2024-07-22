using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public class buildingsDataJSON
    {
        public string role = "building";
        public string type;
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        
    }

    public List<Building> BuildingsList { get; private set; } = new List<Building>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
}