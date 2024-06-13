using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    private static ResourceManager instance;

    [SerializeField] private TextMeshProUGUI woodText;

    private readonly List<IResourceStorage> resourceStorages = new();

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

    public void Update()
    {
        int woodAmount = 0;
        foreach (IResourceStorage storage in resourceStorages)
            woodAmount += storage.GetResourceAmount(ResourceNode.ResourceType.Wood);

        woodText.text = woodAmount.ToString();
    }

    public void RegisterStorage(IResourceStorage storage)
    {
        resourceStorages.Add(storage);
    }
}