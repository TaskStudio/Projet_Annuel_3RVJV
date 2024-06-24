using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "Entities/Entity")]
public class EntityData : ObjectData
{
    public int maxHealthPoints;
    public int maxManaPoints;
    public float attackSpeed;
    public float movementSpeed;
    public string race; 
}