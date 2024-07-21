using UnityEngine;

public class Gatherer : Ally
{
    public ResourceNode resourceNode;
    public ResourceStorage resourceStorage;
    public int maxResourceAmount = 10;
    [SerializeField] private float gatheringDistance = 1.5f;
    private Resource carriedResource;
    private bool gatheringResources;

    protected new void Update()
    {
        base.Update();

        if (gatheringResources) HandleGatheringBehavior();
    }

    public override void SetTarget(Entity target)
    {
        if (target is ResourceNode)
        {
            resourceNode = target as ResourceNode;
            resourceStorage = FindNearestResourceStorage();
            gatheringResources = true;
        }
        else
        {
            resourceNode = null;
            resourceStorage = null;
            gatheringResources = false;
        }
    }

    private void HandleGatheringBehavior()
    {
        if (carriedResource == null || carriedResource?.amount < maxResourceAmount)
        {
            if (resourceNode != null && !resourceNode.isDepleted)
            {
                Move(resourceNode.transform.position);
                if (Vector3.Distance(transform.position, resourceNode.transform.position) <= gatheringDistance)
                {
                    Stop();
                    GatherResource();
                }
            }
            else
            {
                Move(resourceStorage.transform.position);
                if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= gatheringDistance)
                {
                    Stop();
                    DepositResource();
                }
            }
        }
        else
        {
            if (resourceStorage == null) return;
            Move(resourceStorage.transform.position);
            if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= gatheringDistance)
            {
                Stop();
                DepositResource();
            }
        }
    }

    private ResourceStorage FindNearestResourceStorage()
    {
        ResourceStorage[] storages = FindObjectsOfType<ResourceStorage>();
        ResourceStorage nearestStorage = null;
        float minDistance = float.MaxValue;

        foreach (ResourceStorage storage in storages)
        {
            float distance = Vector3.Distance(transform.position, storage.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestStorage = storage;
            }
        }

        return nearestStorage;
    }

    private void GatherResource()
    {
        //a code that use my Ressource node code to make the gatherer gather ressource
        if (resourceNode != null && !resourceNode.isDepleted)
            carriedResource =
                resourceNode.GatherResource(maxResourceAmount - (carriedResource?.amount ?? 0));
    }

    private void DepositResource()
    {
        if (resourceStorage != null && carriedResource != null)
        {
            resourceStorage.AddResource(carriedResource);
            StatManager.IncrementResources(carriedResource);
            
            carriedResource = null;
        }
    }
}