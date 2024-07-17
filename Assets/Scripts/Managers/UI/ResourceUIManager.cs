using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceUIManager : MonoBehaviour
{
    public static ResourceUIManager Instance;

    private VisualElement resourcesContainer;

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
        resourcesContainer = rootVisualElement.Q<VisualElement>("ResourcesContainer");

        if (resourcesContainer == null)
        {
            Debug.LogError("ResourcesContainer not found in the UXML. Check the UXML and the names.");
            return;
        }

        // Initial update based on ResourceManager data
        UpdateResourceUI(ResourceManager.Instance.GetResources());
    }

    public void UpdateResourceUI(Dictionary<Resource.Type, int> resources)
    {
        resourcesContainer.Clear();

        foreach (var resource in resources)
        {
            var resourceLabel = new Label { text = $"{resource.Key}: {resource.Value}" };
            resourcesContainer.Add(resourceLabel);
        }
    }
}