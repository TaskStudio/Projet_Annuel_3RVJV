using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewEntity : BaseObject
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;

    public Profile profile; // Assign the profile in the inspector

    public int hp { get; }
}