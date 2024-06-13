using UnityEngine;

public class ResourceStorage : MonoBehaviour, IResourceStorage
{
    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
    }

    public void AddResource(Resource resource)
    {
        resourceManager.RegisterResource(resource);
    }
}