using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private readonly List<BaseObject> selectedProfiles = new();

    public VisualElement minimapContainer;
    public VisualElement faceContainer;
    public VisualElement selectedEntitiesList;
    public VisualElement statisticsScrollView;
    public VisualElement characterPanel;
    public VisualElement selectedPanel;
    public VisualElement resoursesPanel;
    
    public static UIManager Instance { get; private set; }

    private bool isMouseOverUI;

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
        if (rootVisualElement == null)
        {
            Debug.LogError("Root Visual Element is null. Ensure UIDocument component is set up correctly.");
            return;
        }

        minimapContainer = rootVisualElement.Q<VisualElement>("MiniMap");
        selectedEntitiesList = rootVisualElement.Q<VisualElement>("SelectedEntitiesList");
        statisticsScrollView = rootVisualElement.Q<VisualElement>("StatisticsScrollView");
        faceContainer = rootVisualElement.Q<VisualElement>("FaceContainer");
        characterPanel = rootVisualElement.Q<VisualElement>("Character");
        selectedPanel = rootVisualElement.Q<VisualElement>("Selected");
        resoursesPanel = rootVisualElement.Q<VisualElement>("Resources");

        if (selectedEntitiesList == null || statisticsScrollView == null || faceContainer == null || characterPanel == null || selectedPanel == null)
        {
            Debug.LogError("Containers are not found in the UXML. Check the UXML and the names.");
        }

        // Register hover event handlers for UI elements
        RegisterHoverEvents(minimapContainer);
        RegisterHoverEvents(characterPanel);
        RegisterHoverEvents(selectedPanel);
        RegisterHoverEvents(resoursesPanel);
        
        // Initialize empty panels at start
        characterPanel.style.display = DisplayStyle.None;
        selectedPanel.style.display = DisplayStyle.None;
    }

    private void RegisterHoverEvents(VisualElement element)
    {
        element.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        element.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
    }

    private void OnPointerEnter(PointerEnterEvent evt)
    {
        isMouseOverUI = true;
        Debug.Log("Pointer entered: " + evt.target);
    }

    private void OnPointerLeave(PointerLeaveEvent evt)
    {
        isMouseOverUI = false;
        Debug.Log("Pointer left: " + evt.target);
    }

    public bool IsMouseOverUI()
    {
        return isMouseOverUI;
    }

    private Texture2D GetProfileImage(BaseObject profile)
    {
        if (profile is BaseObject objectProfile) return objectProfile.Data.image;
        return null;
    }

    public void UpdateSelectedEntities(List<BaseObject> newSelectedProfiles)
    {
        selectedProfiles.Clear();
        selectedProfiles.AddRange(newSelectedProfiles);

        UpdateSelectedEntitiesList();
        var selectedProfile = selectedProfiles.Count > 0 ? selectedProfiles[0] : null;
        UpdateFirstSelectedEntity(selectedProfile); // Display the face of the first entity selected 
        UpdateStatisticsContainer(selectedProfile); // Display the stats of the first entity selected 

        // Update visibility of #Character and #Selected panels
        bool hasSelectedEntities = selectedProfiles.Count > 0;
        characterPanel.style.display = hasSelectedEntities ? DisplayStyle.Flex : DisplayStyle.None;
        selectedPanel.style.display = hasSelectedEntities ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void UpdateFirstSelectedEntity(BaseObject profile)
    {
        faceContainer.Clear();
        if (profile == null) return;

        if (profile is BaseObject entityProfile)
        {
            var image = new Image { image = GetProfileImage(profile) };
            faceContainer.Add(image);
        }
    }

    private void UpdateSelectedEntitiesList()
    {
        selectedEntitiesList.Clear();

        for (int i = 0; i < selectedProfiles.Count; i++)
        {
            var profile = selectedProfiles[i];
            var image = new Image { image = GetProfileImage(profile) };
            image.AddToClassList("selectedEntities");

            // Add the class only to the first element
            if (i == 0) image.AddToClassList("selectedEntity");

            selectedEntitiesList.Add(image);
        }
    }

    private void UpdateStatisticsContainer(BaseObject profile)
    {
        statisticsScrollView.Clear();
        if (profile == null) return;

        var nameLabel = new Label { text = profile.Data.objectName };
        var descriptionLabel = new Label { text = profile.Data.description };

        statisticsScrollView.Add(nameLabel);
        statisticsScrollView.Add(descriptionLabel);

        if (profile is Unit unit)
        {
            UnitData unitData = (UnitData) unit.Data;
            var hpLabel = new Label { text = "HP : " + unit.currentHealth };
            var manaLabel = new Label { text = "Mana : " + unit.currentMana };
            var attackSpeedLabel = new Label { text = "Attack Speed : " + unit.attackSpeed };
            var movementSpeedLabel = new Label { text = "Movement Speed : " + unit.movementSpeed };
            var raceLabel = new Label { text = "Race : " + unitData.faction };

            statisticsScrollView.Add(hpLabel);
            statisticsScrollView.Add(manaLabel);
            statisticsScrollView.Add(attackSpeedLabel);
            statisticsScrollView.Add(movementSpeedLabel);
            statisticsScrollView.Add(raceLabel);
        }
    }
}