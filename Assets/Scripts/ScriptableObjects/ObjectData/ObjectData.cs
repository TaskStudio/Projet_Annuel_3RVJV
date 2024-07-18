using UnityEngine;

public abstract class ObjectData : ScriptableObject
{
    [Header("Object Data")]
    public string objectName;
    public string description;
    public Texture2D image;

    public string ObjectName => objectName;
    public string Description => description;
    public Texture2D Image => image;
}