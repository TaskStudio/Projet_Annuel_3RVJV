using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class PrefabInstantiator : MonoBehaviour
{
    public TextAsset jsonFile; // Assign in the Inspector
    void Start()
    {
        ObjectList objectList = JsonUtility.FromJson<ObjectList>(jsonFile.text);
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
}