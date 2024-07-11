public class ResourceStorage : Building
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