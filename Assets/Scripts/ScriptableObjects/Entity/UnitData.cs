using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "ObjectData/UnitData")]
public class UnitData : EntityData
{
    [Space(10)]
    [Header("Unit Data")]
    public int maxManaPoints;
    public float attackSpeed = 1f;
    public float movementSpeed = 5f;
}