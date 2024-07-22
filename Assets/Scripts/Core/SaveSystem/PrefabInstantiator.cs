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
        if (jsonFile == null) return;
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
                GameObject obj = Addressables.InstantiateAsync(objData.addressableKey).WaitForCompletion();
                BaseObject baseObject = obj.GetComponent<BaseObject>();
                if (baseObject is Building)
                {
                    PlacementSystem.Instance.PlaceBuildingAtLocation(
                        obj.GetComponent<Building>(),
                        objData.position,
                        objData.size
                    );
                }
                else if (baseObject is Unit)
                {
                    obj.transform.position = objData.position;
                    obj.transform.rotation = objData.rotation;
                }
            }
        }
        else
        {
            Debug.LogError("No JSON file found.");
        }
    }
}