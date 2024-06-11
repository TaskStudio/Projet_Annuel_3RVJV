using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceProfile", menuName = "Profiles/ResourceProfile")]
public class ResourceProfile : ScriptableObject, IProfile
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ResourceType;
}