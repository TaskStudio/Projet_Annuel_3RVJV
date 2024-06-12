using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour, IResourceStorage {
    public Dictionary<ResourceNode.ResourceType, int> storedResources = new Dictionary<ResourceNode.ResourceType, int>();

    public void AddResource(ResourceNode.ResourceType type, int amount) {
        if (!storedResources.ContainsKey(type)) {
            storedResources[type] = 0;
        }
        storedResources[type] += amount;
    }

    public int GetResourceAmount(ResourceNode.ResourceType type) {
        if (storedResources.ContainsKey(type)) {
            return storedResources[type];
        }
        return 0;
    }
}