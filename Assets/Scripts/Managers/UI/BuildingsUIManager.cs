using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingsUIManager : MonoBehaviour
{
    public static BuildingsUIManager Instance;

    [SerializeField] private List<BuildingDatabaseSO> buildingDatabases;
    [SerializeField] private PlacementSystem placementSystem;
    private VisualElement buildingActionsContainer;
    private VisualElement buildingsButtonsContainer;

    private Button lastClickedButton;

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
        buildingsButtonsContainer = rootVisualElement.Q<VisualElement>("BuildingsButtonsContainer");
        buildingActionsContainer = rootVisualElement.Q<VisualElement>("BuildingActionsContainer");

        if (buildingsButtonsContainer == null || buildingActionsContainer == null)
        {
            Debug.LogError("BuildingsButtonsContainer or BuildingActionsContainer not found in the UXML. Check the UXML and the names.");
            return;
        }

        // Hide the BuildingActionsContainer by default
        buildingActionsContainer.style.display = DisplayStyle.None;

        CreateBuildingDatabaseButtons();
    }

    private void CreateBuildingDatabaseButtons()
    {
        buildingsButtonsContainer.Clear();

        foreach (var buildingDatabase in buildingDatabases)
        {
            var databaseButton = new Button { text = buildingDatabase.databaseName };
            databaseButton.AddToClassList("buildingButton");
            databaseButton.clicked += () => OnDatabaseButtonClicked(buildingDatabase, databaseButton);
            buildingsButtonsContainer.Add(databaseButton);
        }
    }

    private void OnDatabaseButtonClicked(BuildingDatabaseSO buildingDatabase, Button clickedButton)
    {
        buildingActionsContainer.Clear();

        // Remove buildingButtonClicked class from the last clicked button
        if (lastClickedButton != null)
        {
            lastClickedButton.RemoveFromClassList("buildingButtonClicked");
            lastClickedButton.AddToClassList("buildingButton");
        }

        // Add buildingButtonClicked class to the currently clicked button
        clickedButton.RemoveFromClassList("buildingButton");
        clickedButton.AddToClassList("buildingButtonClicked");
        lastClickedButton = clickedButton;

        // Show the BuildingActionsContainer
        buildingActionsContainer.style.display = DisplayStyle.Flex;

        foreach (var buildingData in buildingDatabase.buildingsData)
        {
            var buildingButton = new Button { text = buildingData.DisplayName };
            buildingButton.AddToClassList("actionButton");

            buildingButton.clicked += () => OnBuildingButtonClicked(buildingData);
            buildingActionsContainer.Add(buildingButton);
        }
    }

    private void OnBuildingButtonClicked(BuildingData buildingData)
    {
        placementSystem.StartPlacement(buildingData);
    }

    public void ClearButtonSelection()
    {
        if (lastClickedButton != null)
        {
            lastClickedButton.RemoveFromClassList("buildingButtonClicked");
            lastClickedButton.AddToClassList("buildingButton");
            lastClickedButton = null;

            // Hide the BuildingActionsContainer
            buildingActionsContainer.style.display = DisplayStyle.None;
        }
    }
}