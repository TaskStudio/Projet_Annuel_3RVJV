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
    private VisualElement costWindow;
    private Label goldCostLabel;
    private Label stoneCostLabel;
    private Label woodCostLabel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        actionsContainer = rootVisualElement.Q<VisualElement>("ActionsContainer");
        costWindow = rootVisualElement.Q<VisualElement>("CostWindow");
        woodCostLabel = costWindow.Q<Label>("WoodCost");
        stoneCostLabel = costWindow.Q<Label>("StoneCost");
        goldCostLabel = costWindow.Q<Label>("GoldCost");

        if (actionsContainer == null || costWindow == null)
        {
            Debug.LogError("ActionsContainer or CostWindow not found in the UXML. Check the UXML and the names.");
            return;
        }

        // Hide the actions container and cost window initially
        actionsContainer.style.display = DisplayStyle.None;
        costWindow.style.display = DisplayStyle.None;

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
                    Debug.Log($"Entity Data Found: {actions[i]}");
                    var entityAction = actions[i];
                    actionButton.RegisterCallback<MouseEnterEvent>(evt => ShowCostWindow(entityAction));
                    actionButton.RegisterCallback<MouseLeaveEvent>(evt => HideCostWindow());
                    actionButton.clicked += () => OnActionButtonClicked(i);
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

    private void ShowCostWindow(EntityAction entityData)
    {
        costWindow.style.display = DisplayStyle.Flex;

        woodCostLabel.text = entityData.woodCost.ToString();
        stoneCostLabel.text = entityData.stoneCost.ToString();
        goldCostLabel.text = entityData.goldCost.ToString();
    }

    private void HideCostWindow()
    {
        costWindow.style.display = DisplayStyle.None;
    }
}