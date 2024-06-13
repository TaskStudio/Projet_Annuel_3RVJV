using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityProfile", menuName = "Profiles/EntityProfile")]
public class EntityProfile : ScriptableObject, IProfile, IDamageable, IProgressable
{
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private int health;

    public string Name => name;
    public string Description => description;
    public int Health => health;
    public int MaxValue => health;

    private int currentHealth;

    public int CurrentValue
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    public int Mana;
    public Texture2D Image;
    public float PhysicalResistance;
    public float MagicalResistance;
    public float AttackSpeed;
    public float MovementSpeed;

    public void Initialize()
    {
        currentHealth = health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void UpdateProgress(int value)
    {
        currentHealth = value;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth > health) currentHealth = health;
    }
}