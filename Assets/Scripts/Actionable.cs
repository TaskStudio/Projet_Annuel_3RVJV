using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Actionable : MonoBehaviour, ISelectable
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;

    [Space(10)]
    [SerializeField] private GameObject objectModel;


    // ISelectable implementation
    public bool IsSelected { get; set; }

    public void Select()
    {
        objectModel.layer = LayerMask.NameToLayer("Outlined");
    }

    public void Deselect()
    {
        objectModel.layer = LayerMask.NameToLayer("Default");
    }

    public IProfile GetProfile()
    {
        return null;
    }

    public void UpdateVisuals()
    {
    }
}