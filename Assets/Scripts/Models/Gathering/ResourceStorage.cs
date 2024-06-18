using UnityEngine;

public class ResourceStorage : MonoBehaviour
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