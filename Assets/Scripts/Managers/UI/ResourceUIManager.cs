using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceUIManager : MonoBehaviour
{
    public static ResourceUIManager Instance;

    private static readonly Dictionary<Resource.Type, int> totalResources = new();
    private VisualElement resourcesContainer;
    
    public int startingWood;

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
        totalResources.Add(Resource.Type.Wood, startingWood);
        totalResources.Add(Resource.Type.Stone, 0);
        totalResources.Add(Resource.Type.Gold, 0);

        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        resourcesContainer = rootVisualElement.Q<VisualElement>("ResourcesContainer");

        if (resourcesContainer == null)
        {
            Debug.LogError("ResourcesContainer not found in the UXML. Check the UXML and the names.");
            return;
        }

        UpdateResourceUI();
    }

    public void UpdateResourceUI()
    {
        resourcesContainer.Clear();

        foreach (var resource in totalResources)
        {
            var resourceLabel = new Label { text = $"{resource.Key}: {resource.Value}" };
            resourcesContainer.Add(resourceLabel);
        }
    }

    public static void RegisterResource(Resource resource)
    {
        totalResources[resource.type] += resource.amount;
        Instance.UpdateResourceUI();
    }

    public static bool RequestResource(Resource requestedResource)
    {
        if (totalResources[requestedResource.type] < requestedResource.amount) return false;

        totalResources[requestedResource.type] -= requestedResource.amount;
        Instance.UpdateResourceUI();
        return true;
    }
}