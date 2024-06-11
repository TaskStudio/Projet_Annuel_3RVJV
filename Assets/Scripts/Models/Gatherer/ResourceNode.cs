using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Wood,
        Gold,
        Stone
    }

    public ResourceType resourceType;
    public int resourceAmount = 100;
    public bool isDepleted = false;

    public int GatherResource(int amountRequested)
    {
        int amountGathered = Mathf.Min(amountRequested, resourceAmount);
        resourceAmount -= amountGathered;
        if (resourceAmount <= 0)
        {
            isDepleted = true;
        }
        return amountGathered;
    }
}
