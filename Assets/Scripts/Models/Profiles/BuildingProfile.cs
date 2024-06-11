using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingProfile : IProfile
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Texture2D Image { get; set; }
    public List<EntityProfile> ProducesEntities { get; set; }
}
