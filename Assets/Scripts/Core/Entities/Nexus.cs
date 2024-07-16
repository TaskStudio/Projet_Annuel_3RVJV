using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Nexus : Entity<EntityData>
{
    protected override void Die()
    {
        Debug.Log("Nexus is dead");
    }
}