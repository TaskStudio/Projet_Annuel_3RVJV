using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class Building : Entity
{
    public enum BuildingStates
    {
        Preview,
        Constructing,
        Constructed
    }

    [Header("Building")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Material previewInvalidMaterial;
    [SerializeField] private Material buildingMaterial;
    [Space(5)]
    [SerializeField] private MeshRenderer objectRenderer;
    // public MonoBehaviour behavior;


    [Space(10)] [Header("Grid")]
    [SerializeField] private Material gridMaterial;
    [SerializeField] private Material gridInvalidMaterial;
    [Space(5)]
    [SerializeField] private MeshRenderer gridRenderer;

    private float constructionTime;
    public BuildingStates state { get; internal set; }

    public void Update()
    {
        if (state == BuildingStates.Constructing)
        {
            constructionTime -= Time.deltaTime;
            if (constructionTime <= 0)
                FinishConstruction();
        }
    }

    public void SetGridCellSize(float cellSize)
    {
        gridMaterial.SetVector("_Size", new Vector4(1.0f / cellSize, 1.0f / cellSize, 0, 0));
        gridInvalidMaterial.SetVector("_Size", new Vector4(1.0f / cellSize, 1.0f / cellSize, 0, 0));
    }


    internal void PreviewValid()
    {
        state = BuildingStates.Preview;
        objectRenderer.materials = new[] { previewMaterial };
        gridRenderer.materials = new[] { gridMaterial };
        objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
        objectRenderer.receiveShadows = false;
    }

    internal void PreviewInvalid()
    {
        state = BuildingStates.Preview;
        objectRenderer.materials = new[] { previewInvalidMaterial };
        gridRenderer.materials = new[] { gridInvalidMaterial };
        objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
        objectRenderer.receiveShadows = false;
    }

    internal void StartConstruction(float constructionTime)
    {
        state = BuildingStates.Constructing;
        this.constructionTime = constructionTime;
        gridRenderer.enabled = false;
    }

    internal void FinishConstruction()
    {
        state = BuildingStates.Constructed;
        objectRenderer.materials = new[] { buildingMaterial };
        objectRenderer.shadowCastingMode = ShadowCastingMode.On;
        objectRenderer.receiveShadows = true;
    }

    public override void TargetIsDead(IEntity entity)
    {
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}