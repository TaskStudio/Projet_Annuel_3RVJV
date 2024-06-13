using UnityEngine;

public class Zone : MonoBehaviour, IZone
{
    public Zone(string name, Vector3 position)
    {
        Name = name;
        Position = position;
    }

    public Vector3 Position { get; }
    public string Name { get; }
}