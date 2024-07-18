using System;
using UnityEditor;
using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{
    [SerializeField] private ObjectData data;

    [Space(5)] [Header("Visuals")]
    [SerializeField] private GameObject model;

    public ObjectData Data => data;

    public bool isSelected { get; private set; }

    public bool IsSelected
    {
        get => isSelected;
        private set
        {
            isSelected = value;
            UpdateVisuals();
        }
    }

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


    private void UpdateVisuals()
    {
        model.layer = LayerMask.NameToLayer(isSelected ? "Selected" : "Default");
    }
}