using UnityEngine;

[System.Serializable]
public class EntityProfile : IProfile
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int HealthPoints { get; set; }
    public int Mana { get; set; }
    public Texture2D Image { get; set; }
    public float PhysicalResistance { get; set; }
    public float MagicalResistance { get; set; }
    public float AttackSpeed { get; set; }
    public float MovementSpeed { get; set; }
}