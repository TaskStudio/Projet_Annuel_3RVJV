using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public Resource.Type resourceType;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material depletedMaterial;
    public int resourceAmount = 100;
    public bool isDepleted { get; private set; }

    private void Start()
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
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
}