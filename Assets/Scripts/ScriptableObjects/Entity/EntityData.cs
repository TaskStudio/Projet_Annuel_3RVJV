using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "ObjectData/Entity")]
public class EntityData : ObjectData
{
    [Space(10)]
    [Header("Entity Data")]
    public int maxHealthPoints;
    public FactionEnum faction;
}