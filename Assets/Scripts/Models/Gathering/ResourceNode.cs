using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Wood,
        Gold,
        Stone
    }

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material depletedMaterial;

    public ResourceType resourceType;
    public int resourceAmount = 100;
    public bool isDepleted;

    private void Start()
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public int GatherResource(int amountRequested)
    {
        int amountGathered = Mathf.Min(amountRequested, resourceAmount);
        resourceAmount -= amountGathered;
        if (resourceAmount <= 0)
        {
            isDepleted = true;
            GetComponent<MeshRenderer>().material = depletedMaterial;
        }

        return amountGathered;
    }
}