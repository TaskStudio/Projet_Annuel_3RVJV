using UnityEngine;

public class Gatherer : NonEnemy
{
    public ResourceNode resourceNode;
    public ResourceStorage resourceStorage;
    public int maxResourceAmount = 10;
    private int currentResourceAmount;
    private bool gatheringResources;

    protected override void Update()
    {
        HandleInput();
        base.Update();

        if (gatheringResources) HandleGatheringBehavior();
    }

    protected override void HandleInput()
    {
        if (Input.GetMouseButtonDown(1) && IsSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("ResourceNode"))
                {
                    gatheringResources = true;
                    resourceNode = hit.collider.GetComponent<ResourceNode>();
                    resourceStorage = FindNearestResourceStorage();
                }
                else
                {
                    gatheringResources = false;
                    Move(hit.point);
                }
            }
        }
    }

    public override void Shoot(Vector3 target)
    {
        // Do nothing, Gatherer doesn't shoot
    }

    private void HandleGatheringBehavior()
    {
        if (currentResourceAmount < maxResourceAmount)
        {
            if (resourceNode != null && !resourceNode.isDepleted)
            {
                Move(resourceNode.transform.position);
                if (Vector3.Distance(transform.position, resourceNode.transform.position) <= stoppingDistance)
                    GatherResource();
            }
            else
            {
                Move(resourceStorage.transform.position);
                if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= stoppingDistance)
                    DepositResource();
            }
        }
        else
        {
            Move(resourceStorage.transform.position);
            if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= stoppingDistance)
                DepositResource();
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
        {
            int gatheredAmount = resourceNode.GatherResource(maxResourceAmount - currentResourceAmount);
            currentResourceAmount += gatheredAmount;
        }
    }

    private void DepositResource()
    {
        if (resourceStorage != null)
        {
            resourceStorage.AddResource(resourceNode.resourceType, currentResourceAmount);
            currentResourceAmount = 0;
        }
    }
}