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
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                if (hit.collider.CompareTag("ResourceNode"))
                {
                    // Debug.Log("Resource node identified");
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
        // Debug.Log("Current resource amount: " + currentResourceAmount);
        // Debug.Log("Max resource amount: " + maxResourceAmount);
        // Debug.Log("Resource node: " + (resourceNode == null ? "null" : "not null"));
        // Debug.Log("Resource node depleted: " + (resourceNode != null ? resourceNode.isDepleted.ToString() : "N/A"));

        if (currentResourceAmount < maxResourceAmount)
        {
            if (resourceNode != null && !resourceNode.isDepleted)
            {
                // Debug.Log("Moving to resource node");
                Move(resourceNode.transform.position);
                if (Vector3.Distance(transform.position, resourceNode.transform.position) <= stoppingDistance)
                {
                    Debug.Log("Gathering resources from node");
                    GatherResource();
                }
            }
            else
            {
                // Debug.Log("Moving to resource storage");
                Move(resourceStorage.transform.position);
                if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= stoppingDistance)
                {
                    Debug.Log(
                        "Depositing resources, current : "
                        + resourceStorage.GetResourceAmount(ResourceNode.ResourceType.Wood)
                    );
                    DepositResource();
                }
            }
        }
        else
        {
            // Debug.Log("Moving to resource storage");
            Move(resourceStorage.transform.position);
            if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= stoppingDistance)
            {
                Debug.Log("Depositing resources");
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
        {
            int gatheredAmount = resourceNode.GatherResource(maxResourceAmount - currentResourceAmount);
            currentResourceAmount += gatheredAmount;
            Debug.Log(gatheredAmount);
            Debug.Log($"Gathered {gatheredAmount} resources. Current amount: {currentResourceAmount}");
            if (gatheredAmount == 0 && resourceNode.isDepleted) Debug.Log("Resource node is depleted.");
        }
    }

    private void DepositResource()
    {
        if (resourceStorage != null)
        {
            resourceStorage.AddResource(resourceNode.resourceType, currentResourceAmount);
            Debug.Log(resourceStorage.GetResourceAmount(resourceNode.resourceType));
            Debug.Log($"Deposited {currentResourceAmount} resources of type {resourceNode.resourceType}");
            currentResourceAmount = 0;
        }
        // Debug.Log("Resource storage is null.");
    }
}