using System.Collections;
using System.Collections.Generic;
using Maps.Interfaces;
using UnityEngine;

namespace Maps.Classes
{
    public class Zone : MonoBehaviour, IZone
    {
        public Vector3 Position { get; private set; }
        public string Name { get; private set; }
    
        public Zone(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }
    }

}
