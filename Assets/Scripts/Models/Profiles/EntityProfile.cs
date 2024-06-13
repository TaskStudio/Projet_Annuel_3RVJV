using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityProfile", menuName = "Profiles/EntityProfile")]
public class EntityProfile : ScriptableObject, IProfile, IDamageable
{
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private int health;  // Backing field for Health

    public string Name => name;
    public string Description => description;

    public int Health
    {
        get => health;
        set => health = value;  // Allows setting the Health value
    }

    public int Mana;
    public Texture2D Image;
    public float PhysicalResistance;
    public float MagicalResistance;
    public float AttackSpeed;
    public float MovementSpeed;

    public void TakeDamage(int damage)
    {
        health -= damage; 
        if (health < 0) health = 0;  // Ensure health doesn't go below 0
    }
}