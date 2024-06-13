using UnityEngine;

public class ResourceStorage : MonoBehaviour, IResourceStorage
{
    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
    }

    public void AddResource(ResourceNode.ResourceType type, int amount)
    {
        resourceManager.RegisterResource(type, amount);
    }
}