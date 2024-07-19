using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceUIManager : MonoBehaviour
{
    public static ResourceUIManager Instance;

    private Label woodQuantity;
    private Label stoneQuantity;
    private Label goldQuantity;

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
        woodQuantity = rootVisualElement.Q<Label>("WoodQuantity");
        stoneQuantity = rootVisualElement.Q<Label>("StoneQuantity");
        goldQuantity = rootVisualElement.Q<Label>("GoldQuantity");

        if (woodQuantity == null || stoneQuantity == null || goldQuantity == null)
        {
            Debug.LogError("Labels not found in the UXML. Check the UXML and the names.");
            return;
        }

        // Initial update based on ResourceManager data
        UpdateResourceUI(ResourceManager.Instance.GetResources());
    }

    public void UpdateResourceUI(Dictionary<Resource.Type, int> resources)
    {
        foreach (var resource in resources)
        {
            switch (resource.Key)
            {
                case Resource.Type.Wood:
                    woodQuantity.text = $"{resource.Value}";
                    break;
                case Resource.Type.Stone:
                    stoneQuantity.text = $"{resource.Value}";
                    break;
                case Resource.Type.Gold:
                    goldQuantity.text = $"{resource.Value}";
                    break;
            }
        }
    }
}