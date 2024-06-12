using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityProfile", menuName = "Profiles/EntityProfile")]
public class EntityProfile : ScriptableObject, IProfile
{
    [SerializeField] private new string name;
    [SerializeField] private string description;
    
    public string Name => name;
    public string Description => description;

    public int HealthPoints;
    public int Mana;
    public Texture2D Image;
    public float PhysicalResistance;
    public float MagicalResistance;
    public float AttackSpeed;
    public float MovementSpeed;
}