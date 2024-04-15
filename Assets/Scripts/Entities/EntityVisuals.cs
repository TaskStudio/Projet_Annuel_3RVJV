using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    public GameObject selectionIndicatorPrefab; 
    private GameObject currentIndicatorInstance;

    public void UpdateVisuals(bool isSelected)
    {
        if (isSelected)
        {
            if (currentIndicatorInstance == null)
            {
                currentIndicatorInstance = Instantiate(selectionIndicatorPrefab, transform.position, Quaternion.identity, transform);
                currentIndicatorInstance.transform.localPosition = new Vector3(0, -1f, 0); 
            }
        }
        else
        {
            if (currentIndicatorInstance != null)
            {
                Destroy(currentIndicatorInstance);
            }
        }
    }
}