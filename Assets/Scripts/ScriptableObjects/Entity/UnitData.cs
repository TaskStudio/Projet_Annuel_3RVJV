using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "ObjectData/UnitData")]
public class UnitData : EntityData
{
    public int maxManaPoints;
    public float attackSpeed;
    public float movementSpeed;
}