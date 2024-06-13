using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    private static ResourceManager instance;

    [SerializeField] private TextMeshProUGUI woodText;

    private readonly Dictionary<ResourceNode.ResourceType, int> totalResources = new();


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
        totalResources.Add(ResourceNode.ResourceType.Wood, 0);
    }

    public void Update()
    {
        woodText.text = totalResources[ResourceNode.ResourceType.Wood].ToString();
    }

    public void RegisterResource(ResourceNode.ResourceType type, int amount)
    {
        totalResources.TryAdd(type, 0);
        totalResources[type] += amount;
    }

    public bool RequestResource(ResourceNode.ResourceType type, int cost)
    {
        if (totalResources[type] < cost) return false;

        totalResources[type] -= cost;
        return true;
    }
}