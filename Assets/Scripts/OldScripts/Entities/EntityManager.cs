using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    private List<Entityy> entities = new List<Entityy>();

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
    

    public void RegisterEntity(Entityy entity)
    {
        if (!entities.Contains(entity))
        {
            entities.Add(entity);
        }
    }

    public void UnregisterEntity(Entityy entity)
    {
        entities.Remove(entity);
    }
    
}