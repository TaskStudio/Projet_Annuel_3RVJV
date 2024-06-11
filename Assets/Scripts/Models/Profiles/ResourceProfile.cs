using UnityEngine;

[System.Serializable]
public class ResourceProfile : IProfile
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Texture2D Image { get; set; }
    public string ResourceType { get; set; }
}
