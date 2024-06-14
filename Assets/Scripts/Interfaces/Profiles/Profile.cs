using UnityEngine;

public class Profile : ScriptableObject
{
    public int health;
    public string Name { get; }
    public string Description { get; }
    public int CurrentValue { get; set; }
    public int MaxValue => health;

    public void Initialize()
    {
        CurrentValue = health;
    }

    public void UpdateProgress(int value)
    {
        CurrentValue = value;
        if (CurrentValue < 0) CurrentValue = 0;
        if (CurrentValue > health) CurrentValue = health;
    }
}