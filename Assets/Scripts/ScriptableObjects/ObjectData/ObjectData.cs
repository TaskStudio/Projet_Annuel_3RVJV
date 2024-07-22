using UnityEngine;

public class ObjectData : ScriptableObject
{
    [Header("Object Data")] public string objectName = "N/A";

    public string description = "N/A";
    public Texture2D image;

    [Space(10)] [Header("Entity Data")] public FactionEnum faction;

    public int maxHealthPoints = 10;

    [Space(10)] [Header("Unit Data")] public int maxManaPoints;

    public float attackSpeed = 1f;
    public float movementSpeed = 5f;

    [Space(10)] [Header("Fighter Data")] public int attackDamage = 10;

    public Fighter.DistanceType distanceType;
    public int detectionRange = 15;
    public int attackRange = 5;
    public float attackCooldown = 0.5f;
}