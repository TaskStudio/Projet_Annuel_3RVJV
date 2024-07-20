using UnityEngine;

public class ResourceNode : Entity
{
    public Resource.Type resourceType;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material depletedMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    public int resourceAmount = 100;
    public bool isDepleted { get; private set; }

    private void Start()
    {
        meshRenderer.material = defaultMaterial;
    }

    public Resource GatherResource(int amountRequested)
    {
        int amountGathered = Mathf.Min(amountRequested, resourceAmount);
        resourceAmount -= amountGathered;
        if (resourceAmount <= 0)
        {
            isDepleted = true;
            GetComponent<MeshRenderer>().material = depletedMaterial;
        }

        return new Resource(resourceType, amountGathered);
    }

    protected override void Initialize()
    {
    }

    protected override void Die()
    {
    }
}