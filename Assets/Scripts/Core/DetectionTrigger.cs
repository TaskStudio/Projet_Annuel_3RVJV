using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
    [SerializeField] private Fighter fighter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBase"))
        {
            IEntity entity = other.GetComponent<IEntity>();
            if (entity != null) fighter.AddTargetInRange(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBase"))
        {
            IEntity entity = other.GetComponent<IEntity>();
            if (entity != null) fighter.RemoveTargetInRange(entity);
        }
    }
}