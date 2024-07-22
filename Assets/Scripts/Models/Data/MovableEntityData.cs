using System;
using UnityEngine;

[Serializable]
public class MovableEntityData
{
    public string role = "unit";
    public string type;
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string path;
}