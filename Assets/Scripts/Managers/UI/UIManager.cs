using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public RawImage minimapRawImage;
    public UIDocument resourcesDocument;
    public BuildingsUIManager buildingsUIManager;

    public bool mapEditorContext;

    private readonly List<BaseObject> selectedProfiles = new();

    public VisualElement actionsPanel;
    public VisualElement buildingActionsContainer;

    public VisualElement buildingList;
    public VisualElement characterPanel;
    public VisualElement faceContainer;
    private bool isMouseOverUI;
    public VisualElement resourcesPanel;
    public VisualElement selectedEntitiesList;
    public VisualElement selectedPanel;
    public VisualElement statisticsScrollView;

    public static UIManager Instance { get; private set; }

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

        if (resourcesDocument == null)
        {
            Debug.LogError("Resources Visual Element is null. Ensure UIDocument component is set up correctly.");
            return;
        }

        selectedEntitiesList = rootVisualElement.Q<VisualElement>("SelectedEntitiesList");
        statisticsScrollView = rootVisualElement.Q<VisualElement>("StatisticsScrollView");
        faceContainer = rootVisualElement.Q<VisualElement>("FaceContainer");
        characterPanel = rootVisualElement.Q<VisualElement>("Character");
        selectedPanel = rootVisualElement.Q<VisualElement>("Selected");

        resourcesPanel = resourcesDocument.rootVisualElement.Q<VisualElement>("ResourcesContainer");
        actionsPanel = rootVisualElement.Q<VisualElement>("ActionsContainer");

        buildingList = rootVisualElement.Q<VisualElement>("BuildingList");
        buildingActionsContainer = rootVisualElement.Q<VisualElement>("BuildingActionsContainer");

        if (selectedEntitiesList == null
            || statisticsScrollView == null
            || faceContainer == null
            || characterPanel == null
            || selectedPanel == null
            || resourcesPanel == null
            || actionsPanel == null
            || buildingList == null
            || buildingActionsContainer == null)
        {
            Debug.LogError("Containers are not found in the UXML. Check the UXML and the names.");
            return;
        }

        RegisterHoverEvents(actionsPanel);
        RegisterHoverEvents(resourcesPanel);
        RegisterHoverEvents(characterPanel);
        RegisterHoverEvents(selectedPanel);
        RegisterHoverEvents(buildingList);
        RegisterHoverEvents(buildingActionsContainer);

        characterPanel.style.display = DisplayStyle.None;
        selectedPanel.style.display = DisplayStyle.None;
        buildingList.style.display = DisplayStyle.None;
        buildingActionsContainer.style.display = DisplayStyle.None;

        if (mapEditorContext) buildingList.style.display = DisplayStyle.Flex;

        RegisterRawImageHoverEvents();
    }

    private void RegisterHoverEvents(VisualElement element)
    {
        element.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        element.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
    }

    private void OnPointerEnter(PointerEnterEvent evt)
    {
        isMouseOverUI = true;
    }

    private void OnPointerLeave(PointerLeaveEvent evt)
    {
        isMouseOverUI = false;
    }

    private void RegisterRawImageHoverEvents()
    {
        if (minimapRawImage != null)
        {
            var trigger = minimapRawImage.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = minimapRawImage.gameObject.AddComponent<EventTrigger>();

            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener(eventData => { isMouseOverUI = true; });

            var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener(eventData => { isMouseOverUI = false; });

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
        }
    }

    public bool IsMouseOverUI()
    {
        return isMouseOverUI;
    }

    public void UpdateSelectedEntities(List<BaseObject> newSelectedProfiles)
    {
        SelectedEntityManager.Instance.UpdateSelectedEntities(newSelectedProfiles);

        bool hasSelectedEntities = newSelectedProfiles.Count > 0;

        selectedPanel.style.display = hasSelectedEntities ? DisplayStyle.Flex : DisplayStyle.None;

        if ((hasSelectedEntities && newSelectedProfiles[0] is Nexus) || mapEditorContext)
        {
            buildingList.style.display = DisplayStyle.Flex;
            selectedPanel.style.display = DisplayStyle.None;
            if (mapEditorContext) buildingsUIManager.ClearButtonSelection();
        }
        else
        {
            buildingList.style.display = DisplayStyle.None;
            buildingsUIManager.ClearButtonSelection();
        }
    }
}