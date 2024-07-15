using System;
using UnityEditor;
using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{
    [Space(5)] [Header("Visuals")]
    [SerializeField] private GameObject model;
    public abstract ObjectData Data { get; }

    public bool IsSelected
    {
        get => isSelected;
        private set
        {
            isSelected = value;
            UpdateVisuals();
        }
    }

    public bool isSelected { get; private set; }

    private void OnEnable()
    {
        Initialize();
    }

    protected virtual void OnValidate()
    {
        try
        {
            ValidateData();
            Initialize();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void ValidateData()
    {
#if UNITY_EDITOR
        if (Data == null)
        {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            throw new Exception($"The data property of {gameObject.name} ({GetType().Name}) is null. Path: " + path);
        }
#endif
    }

    protected abstract void Initialize();

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    public void OnHoverEnter()
    {
        model.layer = LayerMask.NameToLayer("Hovered");
    }

    public void OnHoverExit()
    {
        if (!isSelected)
            model.layer = LayerMask.NameToLayer("Default");
        else
            model.layer = LayerMask.NameToLayer("Selected");
    }


    private void UpdateVisuals()
    {
        model.layer = LayerMask.NameToLayer(isSelected ? "Selected" : "Default");
    }
}

public abstract class BaseObject<TDataType> : BaseObject where TDataType : ObjectData
{
    [SerializeField] protected TDataType data;

    public override ObjectData Data => data;
}