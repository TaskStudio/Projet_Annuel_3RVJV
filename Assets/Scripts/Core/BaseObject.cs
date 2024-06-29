using UnityEngine;

/// <summary>
///     Class <c>BaseObject</c> represents a Base Object in the game, that can be selected
/// </summary>
public abstract class BaseObject : MonoBehaviour
{
    [Space(10)] [Header("Visuals")]
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

    public void UpdateVisuals()
    {
        if (IsSelected)
            model.layer = LayerMask.NameToLayer("Outlined");
        else
            model.layer = LayerMask.NameToLayer("Default");
    }
}