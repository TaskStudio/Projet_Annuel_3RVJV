using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MovableEntityManager : MonoBehaviour
{
    public static MovableEntityManager Instance { get; private set; }
    public List<MovableEntityData> MovableEntitiesData { get; } = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // public void AddMovableEntity(Unit entity)
    // {
    //     var data = new MovableEntityData
    //     {
    //         type = entity.GetType().Name,
    //         name = entity.name,
    //         position = entity.transform.position,
    //         rotation = entity.transform.rotation,
    //         path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(entity)
    //     };
    //     MovableEntitiesData.Add(data);
    // }

    public void RemoveMovableEntity(Unit entity)
    {
        MovableEntitiesData.RemoveAll(e => e.name == entity.name);
    }

    // Method to serialize movable entities to JSON
    public void SaveMovableEntitiesToJson(string filePath)
    {
        var json = JsonUtility.ToJson(new { movableEntities = MovableEntitiesData }, true);
        File.WriteAllText(filePath, json);
    }
}