using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{
    public ObjectData data;

    [Space(10)] [Header("Visuals")]
    [SerializeField] private GameObject model;

    private bool isSelected;

    public bool IsSelected
    {
        get => isSelected;
        private set
        {
            isSelected = value;
            UpdateVisuals();
        }
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    private void UpdateVisuals()
    {
        model.layer = LayerMask.NameToLayer(isSelected ? "Outlined" : "Default");
    }
}