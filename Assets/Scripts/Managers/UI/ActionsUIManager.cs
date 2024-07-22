using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionsUIManager : MonoBehaviour
{
    public static ActionsUIManager Instance;
    public bool mapEditorMode;

    [SerializeField] [CanBeNull] private UnitPlacementSystem unitPlacementSystem;

    private VisualElement actionsContainer;

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
        actionsContainer = rootVisualElement.Q<VisualElement>("ActionsContainer");

        if (actionsContainer == null)
        {
            Debug.LogError("ActionsContainer not found in the UXML. Check the UXML and the names.");
            return;
        }

        // Hide the actions container initially
        actionsContainer.style.display = DisplayStyle.None;

        if (mapEditorMode) UpdateActionButtons(new List<EntityAction>());
    }

    public void UpdateActionButtons(List<EntityAction> actions)
    {
        if (mapEditorMode) actions = unitPlacementSystem?.unitsProductionOrders;

        actionsContainer.Clear();

        if (actions?.Count > 0)
        {
            // Show the actions container
            actionsContainer.style.display = DisplayStyle.Flex;

            for (var i = 0; i < actions.Count; i++)
            {
                var actionButton = new Button { text = actions[i].actionName };
                actionButton.AddToClassList("actionButton");
                if (mapEditorMode)
                {
                    actionButton.clicked += actions[i].action.Invoke;
                }
                else
                {
                    var actionIndex = i; // Capture the current index for the callback
                    actionButton.clicked += () => OnActionButtonClicked(actionIndex);
                }

                actionsContainer.Add(actionButton);
            }
        }
        else
        {
            // Hide the actions container if there are no actions
            actionsContainer.style.display = DisplayStyle.None;
        }
    }

    private void OnActionButtonClicked(int actionIndex)
    {
        SelectionManager.Instance.OnInvokeActionable(actionIndex);
    }
}