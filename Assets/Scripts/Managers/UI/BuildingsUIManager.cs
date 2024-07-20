using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingsUIManager : MonoBehaviour
{
    public static BuildingsUIManager Instance;

    [SerializeField] private BuildingDatabaseSO buildingDatabase;
    [SerializeField] private PlacementSystem placementSystem;
    private VisualElement buildingsContainer;

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
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        buildingsContainer = rootVisualElement.Q<VisualElement>("BuildingContainer");

        if (buildingsContainer == null)
        {
            Debug.LogError("BuildingContainer not found in the UXML. Check the UXML and the names.");
            return;
        }

        CreateBuildingButtons();
    }

    private void CreateBuildingButtons()
    {
        buildingsContainer.Clear();

        foreach (BuildingData buildingData in buildingDatabase.buildingsData)
        {
            var buildingButton = new Button { text = buildingData.DisplayName };
            buildingButton.AddToClassList("actionButton");
            buildingButton.clicked += () => OnBuildingButtonClicked(buildingData.IdNumber);
            buildingsContainer.Add(buildingButton);
        }
    }

    private void OnBuildingButtonClicked(int buildingID)
    {
        placementSystem.StartPlacement(buildingID);
    }
}