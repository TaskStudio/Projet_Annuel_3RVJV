using System;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform buildingPivot;

    [Space(10)] [Header("Grid")]
    [SerializeField] private Material gridMaterial;
    [SerializeField] private Material gridInvalidMaterial;
    [Space(5)]
    [SerializeField] private MeshRenderer gridRenderer;

    private BuildingManager buildingManager;
    private float constructionTime;
    public BuildingStates state { get; internal set; }
    public Vector3 pivotOffset { get; private set; }
    public List<Vector3Int> occupiedGridPositions { get; set; }
    public Vector2Int Size { get; set; }

    public void Update()
    {
        if (state == BuildingStates.Constructing)
        {
            constructionTime -= Time.deltaTime;
            if (constructionTime <= 0)
                Place();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        pivotOffset = buildingPivot.position - transform.position;
    }

    public void SetGridCellSize(float cellSize)
    {
        gridMaterial.SetVector("_Size", new Vector4(1.0f / cellSize, 1.0f / cellSize, 0, 0));
        gridInvalidMaterial.SetVector("_Size", new Vector4(1.0f / cellSize, 1.0f / cellSize, 0, 0));
    }


    internal new void PreviewValid()
    {
        base.PreviewValid();
        state = BuildingStates.Preview;
        gridRenderer.materials = new[] { gridMaterial };
    }

    internal new void PreviewInvalid()
    {
        base.PreviewInvalid();
        state = BuildingStates.Preview;
        gridRenderer.materials = new[] { gridInvalidMaterial };
    }

    internal void StartConstruction(float constructionTime)
    {
        state = BuildingStates.Constructing;
        this.constructionTime = constructionTime;
        gridRenderer.enabled = false;
    }

    internal new void Place()
    {
        base.Place();
        state = BuildingStates.Constructed;
        AddToBuildingManager();
    }

    protected override void Initialize()
    {
    }

    public void AddToBuildingManager()
    {
        BuildingManager.Instance.AddBuilding(this);
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}