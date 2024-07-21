using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingsUIManager : MonoBehaviour
{
    public static BuildingsUIManager Instance;

    [SerializeField] private List<BuildingDatabaseSO> buildingDatabases;
    [SerializeField] private PlacementSystem placementSystem;
    private VisualElement buildingsButtonContainer;
    private VisualElement buildingActionsContainer;

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
        buildingsButtonContainer = rootVisualElement.Q<VisualElement>("BuildingsButtonContainer");
        buildingActionsContainer = rootVisualElement.Q<VisualElement>("BuildingActionsContainer");

        if (buildingsButtonContainer == null || buildingActionsContainer == null)
        {
            Debug.LogError("BuildingButtonContainer or BuildingActionsContainer not found in the UXML. Check the UXML and the names.");
            return;
        }

        CreateBuildingDatabaseButtons();
    }

    private void CreateBuildingDatabaseButtons()
    {
        buildingsButtonContainer.Clear();

        foreach (BuildingDatabaseSO buildingDatabase in buildingDatabases)
        {
            var databaseButton = new Button { text = buildingDatabase.databaseName };
            databaseButton.AddToClassList("buildingButton");
            databaseButton.clicked += () => OnDatabaseButtonClicked(buildingDatabase);
            buildingsButtonContainer.Add(databaseButton);
        }
    }

    private void OnDatabaseButtonClicked(BuildingDatabaseSO buildingDatabase)
    {
        buildingActionsContainer.Clear();

        foreach (BuildingData buildingData in buildingDatabase.buildingsData)
        {
            var buildingButton = new Button { text = buildingData.DisplayName };
            buildingButton.AddToClassList("actionButton");

            // Add the building image if available
            if (buildingData.BuildingImage != null)
            {
                var buildingImage = new Image { sprite = buildingData.BuildingImage };
                buildingButton.Add(buildingImage);
            }

            buildingButton.clicked += () => OnBuildingButtonClicked(buildingData.IdNumber);
            buildingActionsContainer.Add(buildingButton);
        }
    }

    private void OnBuildingButtonClicked(int buildingID)
    {
        placementSystem.StartPlacement(buildingID);
    }
}