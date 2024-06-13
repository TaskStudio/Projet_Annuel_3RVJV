public class Resource
{
    public enum Type
    {
        Wood,
        Gold,
        Stone
    }

    public Resource(Type type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    public Type type { get; }
    public int amount { get; }
}