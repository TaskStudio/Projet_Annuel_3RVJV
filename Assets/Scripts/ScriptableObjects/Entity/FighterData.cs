using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "ObjectData/FighterData")]
public class FighterData : UnitData
{
    [Space(10)]
    [Header("Fighter Data")]
    public int attackDamage = 10;
}