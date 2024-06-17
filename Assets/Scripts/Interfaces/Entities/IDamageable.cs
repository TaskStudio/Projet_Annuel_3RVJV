public interface IDamageable
{
    int hp { get; }
    void TakeDamage(int damage);
}