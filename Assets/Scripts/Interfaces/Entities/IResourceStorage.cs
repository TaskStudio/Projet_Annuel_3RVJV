public interface IResourceStorage {
    void AddResource(ResourceNode.ResourceType type, int amount);
    int GetResourceAmount(ResourceNode.ResourceType type);
}