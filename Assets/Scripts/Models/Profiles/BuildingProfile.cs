using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingProfile", menuName = "Profiles/BuildingProfile")]
public class BuildingProfile : Profile
{
    public List<EntityProfile> ProducesEntities;
}