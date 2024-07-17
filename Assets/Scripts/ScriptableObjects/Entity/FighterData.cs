using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "ObjectData/FighterData")]
public class FighterData : UnitData
{
    [Space(10)]
    [Header("Fighter Data")]
    public int attackDamage = 10;
    public Fighter.DistanceType distanceType;
    public float detectionRange = 15f;
    public float attackRange = 5f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;
}