public class ResourceStorage : Building
{
    public void AddResource(Resource resource)
    {
        ResourceManager.Instance.RegisterResource(resource);
    }
}