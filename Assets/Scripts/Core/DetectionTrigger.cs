using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
    [SerializeField] private Fighter fighter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IEntity>() is { } entity) fighter.AddTargetInRange(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IEntity>() is { } entity) fighter.RemoveTargetInRange(entity);
    }
}