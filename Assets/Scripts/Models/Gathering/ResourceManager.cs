using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    private static ResourceManager instance;

    private static readonly Dictionary<Resource.Type, int> totalResources = new();

    [SerializeField] private TextMeshProUGUI woodText;
    public int startingWood;

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

    private void Start()
    {
        totalResources.Add(Resource.Type.Wood, startingWood);
        totalResources.Add(Resource.Type.Stone, 0);
        totalResources.Add(Resource.Type.Gold, 0);
    }

    public void Update()
    {
        woodText.text = totalResources[Resource.Type.Wood].ToString();
    }

    public static void RegisterResource(Resource resource)
    {
        totalResources[resource.type] += resource.amount;
    }

    public static bool RequestResource(Resource requestedResource)
    {
        if (totalResources[requestedResource.type] < requestedResource.amount) return false;

        totalResources[requestedResource.type] -= requestedResource.amount;
        return true;
    }
}