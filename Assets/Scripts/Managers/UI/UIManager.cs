using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public RawImage minimapRawImage;

    public UIDocument resourcesDocument;
    public UIDocument actionsDocument;

    private readonly List<BaseObject> selectedProfiles = new();

    public VisualElement actionsPanel;

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
        actionsPanel = actionsDocument.rootVisualElement.Q<VisualElement>("ActionsContainer");

        if (selectedEntitiesList == null || statisticsScrollView == null || faceContainer == null ||
            characterPanel == null || selectedPanel == null || resourcesPanel == null || actionsPanel == null)
        {
            Debug.LogError("Containers are not found in the UXML. Check the UXML and the names.");
            return;
        }

        RegisterHoverEvents(actionsPanel);
        RegisterHoverEvents(resourcesPanel);
        RegisterHoverEvents(characterPanel);
        RegisterHoverEvents(selectedPanel);


        // Initialize empty panels at start
        characterPanel.style.display = DisplayStyle.None;
        selectedPanel.style.display = DisplayStyle.None;

        // Register RawImage hover events
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
    }
}