using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "ObjectData/Entity")]
public class EntityData : ObjectData
{
    public int maxHealthPoints;
    public string faction;
}