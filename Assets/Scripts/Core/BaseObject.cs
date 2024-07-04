using FishNet.Object;
using UnityEngine;

public abstract class BaseObject : NetworkBehaviour
{
    public string objectName;
    public string description;
    public Texture2D image;
    public ObjectData data;
    
    [Space(10)] [Header("Visuals")]
    [SerializeField] private GameObject model;
    public bool IsSelected { get; set; }

    public virtual void Initialize()
    {
        objectName = data.objectName;
        description = data.description;
        image = data.image;
    }
    
    public string GetName()
    {
        return objectName;
    }

    public string GetDescription()
    {
        return description;
    }

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