using UnityEngine;

public class Gatherer : Unit
{
    public ResourceNode resourceNode;
    public ResourceStorage resourceStorage;
    public int maxResourceAmount = 10;
    [SerializeField] private float gatheringDistance = 1.5f;
    private Resource carriedResource;
    private bool gatheringResources;

    protected override void Update()
    {
        HandleInput();
        base.Update();

        if (gatheringResources) HandleGatheringBehavior();
    }

    protected void HandleInput()
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

    private void HandleGatheringBehavior()
    {
        if (carriedResource == null || carriedResource?.amount < maxResourceAmount)
        {
            if (resourceNode != null && !resourceNode.isDepleted)
            {
                Move(resourceNode.transform.position);
                if (Vector3.Distance(transform.position, resourceNode.transform.position) <= gatheringDistance)
                    GatherResource();
            }
            else
            {
                Move(resourceStorage.transform.position);
                if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= gatheringDistance)
                    DepositResource();
            }
        }
        else
        {
            if (resourceStorage == null) return;
            Move(resourceStorage.transform.position);
            if (Vector3.Distance(transform.position, resourceStorage.transform.position) <= gatheringDistance)
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
            carriedResource =
                resourceNode.GatherResource(maxResourceAmount - (carriedResource?.amount ?? 0));
    }

    private void DepositResource()
    {
        if (resourceStorage != null && carriedResource != null)
        {
            resourceStorage.AddResource(carriedResource);
            carriedResource = null;
        }
    }
}