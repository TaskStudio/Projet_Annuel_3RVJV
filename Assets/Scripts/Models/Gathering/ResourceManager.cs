using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int startingWood;
    public int startingStone;
    public int startingGold;

    private readonly Dictionary<Resource.Type, int> totalResources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        InitializeResources();
        ResourceUIManager.Instance.UpdateResourceUI(totalResources);
    }

    private void InitializeResources()
    {
        totalResources.Add(Resource.Type.Wood, startingWood);
        totalResources.Add(Resource.Type.Stone, startingStone);
        totalResources.Add(Resource.Type.Gold, startingGold);
    }

    public void RegisterResource(Resource resource)
    {
        if (totalResources.ContainsKey(resource.type))
        {
            totalResources[resource.type] += resource.amount;
            ResourceUIManager.Instance.UpdateResourceUI(totalResources);
        }
    }

    public bool RequestResource(Resource requestedResource)
    {
        if (totalResources.ContainsKey(requestedResource.type)
            && totalResources[requestedResource.type] >= requestedResource.amount)
        {
            totalResources[requestedResource.type] -= requestedResource.amount;
            ResourceUIManager.Instance.UpdateResourceUI(totalResources);
            StatManager.IncrementAllyResourcesSpent(requestedResource);
            return true;
        }

        return false;
    }

    public Dictionary<Resource.Type, int> GetResources()
    {
        return new Dictionary<Resource.Type, int>(totalResources);
    }
}