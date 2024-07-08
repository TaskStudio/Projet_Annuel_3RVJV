using UnityEngine;

public abstract class ObjectData : ScriptableObject
{
    [Header("Object Data")]
    public string objectName;
    public string description;
    public Texture2D image;
}