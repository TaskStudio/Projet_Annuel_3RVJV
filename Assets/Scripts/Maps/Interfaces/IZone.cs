using UnityEngine;

namespace Maps.Interfaces
{
    internal interface IZone
    {
        Vector3 Position { get; }
        string Name { get; }
    }
}

