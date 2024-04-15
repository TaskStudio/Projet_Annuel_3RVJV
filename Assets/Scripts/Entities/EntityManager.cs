using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    private List<Entity> entities = new List<Entity>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    

    public void RegisterEntity(Entity entity)
    {
        if (!entities.Contains(entity))
        {
            entities.Add(entity);
        }
    }

    public void UnregisterEntity(Entity entity)
    {
        entities.Remove(entity);
    }
    
}