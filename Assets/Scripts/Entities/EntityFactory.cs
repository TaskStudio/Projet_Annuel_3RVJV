using Entities;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    [SerializeField] private EntityDatabaseSO entityDatabase;

    public Entity CreateEntity(string entityID, Vector3 position)
    {
        Entity entity = Instantiate(entityDatabase.GetEntityPrefab(entityID), position, Quaternion.identity);
        return entity;
    }
}