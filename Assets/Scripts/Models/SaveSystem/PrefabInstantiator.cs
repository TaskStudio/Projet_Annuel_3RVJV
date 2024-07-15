using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]


public class PrefabInstantiator : MonoBehaviour
{
    public string filePath = Application.dataPath + "/Resources/";
    private TextAsset jsonFile; // Assign in the Inspector
    void Start()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
        FileInfo[] files = directoryInfo.GetFiles("*.json");
        FileInfo latestFile = null;
        foreach (var file in files)
        {
            if (latestFile == null || file.LastWriteTime > latestFile.LastWriteTime)
            {
                latestFile = file;
            }
        }

        if (latestFile != null)
        {
            string jsonContent = File.ReadAllText(latestFile.FullName);
            ObjectList objectList = JsonUtility.FromJson<ObjectList>(jsonContent);
            Debug.Log(objectList.objects.Count);
            foreach (var objData in objectList.objects)
            {
                Debug.Log(objData.type);
                Debug.Log(objData.position);
                Debug.Log(objData.rotation);
                string folderPath = objData.role == "unit" ? "Prefabs/Entities/" : "Prefabs/Construction/Buildings/";
                GameObject prefab = Resources.Load<GameObject>(folderPath + objData.type);
                if (prefab != null)
                {
                    Instantiate(prefab, objData.position, objData.rotation);
                }
                else
                {
                    Debug.LogError($"Prefab not found for type '{objData.type}' at path: {folderPath + objData.type}. Ensure the prefab exists in a 'Resources' folder and the path is correct.");
                }
            }
        }
        else
        {
            Debug.LogError("No JSON file found.");
        }
    }
}