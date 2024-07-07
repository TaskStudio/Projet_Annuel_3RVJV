using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
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

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void OnEnable()
    {
        ValidateData();
    }

    protected virtual void OnValidate()
    {
        ValidateData();
    }

    private void ValidateData()
    {
        if (Data == null)
        {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            Debug.LogError($"The data property of {gameObject.name} ({GetType().Name}) is null. Path: " + path, this);
        }
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


    private void UpdateVisuals()
    {
        model.layer = LayerMask.NameToLayer(isSelected ? "Outlined" : "Default");
    }
}

public abstract class BaseObject<TDataType> : BaseObject where TDataType : ObjectData
{
    [SerializeField] protected TDataType data;

    public override ObjectData Data => data;
}