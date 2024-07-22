using System;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class PrefabInstantiator : MonoBehaviour
{
    public string filePath = Application.dataPath + "/Resources/";
    public TextAsset jsonFile;

    private void Start()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
        FileInfo[] files = directoryInfo.GetFiles("*.json");
        FileInfo latestFile = null;
        foreach (var file in files)
            if (latestFile == null || file.LastWriteTime > latestFile.LastWriteTime)
                latestFile = file;

        if (latestFile != null)
        {
            string jsonContent = File.ReadAllText(latestFile.FullName);
            ObjectList objectList = JsonUtility.FromJson<ObjectList>(jsonContent);
            foreach (var objData in objectList.objects)
            {
                GameObject building = Addressables.InstantiateAsync(objData.addressableKey).WaitForCompletion();
                PlacementSystem.Instance.PlaceBuildingAtLocation(
                    building.GetComponent<Building>(),
                    objData.position,
                    objData.size
                );
            }
        }
        else
        {
            Debug.LogError("No JSON file found.");
        }
    }
}