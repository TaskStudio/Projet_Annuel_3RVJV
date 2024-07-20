using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionsUIManager : MonoBehaviour
{
    public static ActionsUIManager Instance;

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
    }

    public void UpdateActionButtons(List<EntityAction> actions)
    {
        actionsContainer.Clear();

        if (actions.Count > 0)
        {
            // Show the actions container
            actionsContainer.style.display = DisplayStyle.Flex;

            for (var i = 0; i < actions.Count; i++)
            {
                var actionButton = new Button { text = actions[i].actionName };
                actionButton.AddToClassList("actionButton");
                var actionIndex = i; // Capture the current index for the callback
                actionButton.clicked += () => OnActionButtonClicked(actionIndex);
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