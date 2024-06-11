using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuildingProfile", menuName = "Profiles/BuildingProfile")]
public class BuildingProfile : ScriptableObject, IProfile
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<EntityProfile> ProducesEntities;
}