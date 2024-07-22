using System.IO;
using UnityEngine;

public class SaveInfoToJson : MonoBehaviour
{
    public string filePath = Application.dataPath + "/Resources/";
    private BuildingManager buildingManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) SaveToJson();
    }

    public void SaveToJson()
    {
        int saveNumber = 1;
        buildingManager = BuildingManager.Instance;

        if (buildingManager == null)
        {
            Debug.LogError("One of the managers is null.");
            return;
        }

        var buildings = buildingManager.BuildingsList;
        var movableEntities = UnitsManager.MovableUnits;

        var objectList = new ObjectList();
        foreach (var building in buildings)
        {
            if (building == null) continue;

            var buildingData = new PrefabObjectData
            {
                role = "building",
                type = building.name.Replace("(Clone)", "").Trim(),
                name = building.name,
                position = building.transform.position,
                rotation = building.transform.rotation,
                addressableKey = building.AddressableKey,
                ID = building.ID,
                size = building.Size
            };
            objectList.objects.Add(buildingData);
        }

        foreach (var entity in movableEntities)
        {
            if (entity == null) continue;

            // Update entity data with current position and rotation
            var entityData = new PrefabObjectData
            {
                role = "unit",
                type = entity.GetType().Name,
                name = entity.name,
                position = entity.transform.position,
                rotation = entity.transform.rotation,
                addressableKey = entity.AddressableKey,
                ID = entity.ID
            };
            objectList.objects.Add(entityData);
        }

        var json = JsonUtility.ToJson(objectList, true);

        var directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        while (File.Exists(filePath + "Save" + saveNumber + ".json")) saveNumber++;
        File.WriteAllText(filePath + "Save" + saveNumber + ".json", json);
    }
}