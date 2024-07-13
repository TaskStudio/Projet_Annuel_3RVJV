public class ResourceStorage : Building
{
    public void AddResource(Resource resource)
    {
        ResourceManager.RegisterResource(resource);
    }
}