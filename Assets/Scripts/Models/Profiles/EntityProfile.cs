using UnityEngine;

namespace Models.Profiles
{
    [System.Serializable]
    public class EntityProfile
    {
        public string Name;
        public int HealthPoints;
        public int Mana;
        public Texture2D Image;
        public float PhysicalResistance;
        public float MagicalResistance;
        public float AttackSpeed;
        public float MovementSpeed;
    }
}