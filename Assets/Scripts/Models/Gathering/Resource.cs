public class Resource
{
    public enum Type
    {
        Wood,
        Stone,
        Gold
    }

    public Type type;
    public int amount;

    public Resource(Type type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}