using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Actionable : MonoBehaviour, ISelectable
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;

    [Space(10)] [Header("Selection")]
    [SerializeField] private GameObject model;

    public bool IsSelected { get; set; }

    public void Select()
    {
        IsSelected = true;
        UpdateVisuals();
    }

    public void Deselect()
    {
        IsSelected = false;
        UpdateVisuals();
    }

    public Profile GetProfile()
    {
        return null;
    }

    public void UpdateVisuals()
    {
        if (IsSelected)
            model.layer = LayerMask.NameToLayer("Outlined");
        else
            model.layer = LayerMask.NameToLayer("Default");
    }
}