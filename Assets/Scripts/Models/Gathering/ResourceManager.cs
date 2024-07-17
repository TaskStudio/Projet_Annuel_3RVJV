using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void RegisterResource(Resource resource)
    {
        ResourceUIManager.RegisterResource(resource);
    }

    public static bool RequestResource(Resource requestedResource)
    {
        return ResourceUIManager.RequestResource(requestedResource);
    }
}