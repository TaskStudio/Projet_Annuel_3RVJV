using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityProfile", menuName = "Profiles/EntityProfile")]
public class EntityProfile : Profile, IProgressable
{
    public float AttackSpeed;
    [SerializeField] private string description;
    public Texture2D Image;
    public float MagicalResistance;

    public int Mana;
    public float MovementSpeed;
    [SerializeField] private string name;
    public float PhysicalResistance;
}