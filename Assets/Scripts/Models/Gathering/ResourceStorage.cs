using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour, IResourceStorage
{
    public Dictionary<ResourceNode.ResourceType, int> storedResources = new();

    public void AddResource(ResourceNode.ResourceType type, int amount)
    {
        storedResources.TryAdd(type, 0);

        storedResources[type] += amount;
    }

    public int GetResourceAmount(ResourceNode.ResourceType type)
    {
        return storedResources.GetValueOrDefault(type, 0);
    }
}