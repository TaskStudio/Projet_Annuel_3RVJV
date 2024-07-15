using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class PrefabInstantiator : MonoBehaviour
{
    public TextAsset jsonFile;

    private void Start()
    {
        ObjectList objectList = JsonUtility.FromJson<ObjectList>(jsonFile.text);
        foreach (var objData in objectList.objects)
        {
            GameObject building = Addressables.InstantiateAsync(objData.addressableKey).WaitForCompletion();
            building.transform.position = objData.position;
            building.transform.rotation = objData.rotation;
        }
    }
}