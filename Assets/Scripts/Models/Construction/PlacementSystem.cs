using System.Collections;
using System.Collections.Generic;
using GameInput;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private MapGrid mapGrid;
    [SerializeField] private MouseControl mouseControl;
    [SerializeField] private float cellSize = 1.0f;

    public bool mapEditionMode;

    private readonly BuildingGridData BuildingGrid = new();
    private bool buildingIsBeingMoved;
    private Grid grid;

    private bool isBuildingSelected;
    private Building selectedBuilding;
    private BuildingData selectedBuildingData;

    public static PlacementSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        mapGrid.SetGridCellSize(cellSize);
        mapGrid.gameObject.SetActive(false);


        isBuildingSelected = false;

        selectedBuilding = null;
        selectedBuildingData = null;
    }

    private void Update()
    {
        if (isBuildingSelected && selectedBuilding.state == Building.BuildingStates.Preview)
        {
            Vector3 worldMousePos = mouseControl.GetCursorMapPosition();
            Vector3Int gridMousePos = grid.WorldToCell(worldMousePos);
            Vector3 gridWorldPos = grid.CellToWorld(gridMousePos);

            selectedBuilding.buildingPivot.position = gridWorldPos;
            selectedBuilding.transform.position =
                selectedBuilding.buildingPivot.position - selectedBuilding.pivotOffset;

            Vector2Int buildingSize = selectedBuildingData.Size;

            bool canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, buildingSize);
            if (!canPlace)
                selectedBuilding.PreviewInvalid();
            else
                selectedBuilding.PreviewValid();
        }
    }

    private void OnEnable()
    {
        grid = mapGrid.Grid;
    }

    public void StartPlacement(BuildingData buildingData)
    {
        CancelPlacement();
        mapGrid.gameObject.SetActive(true);

        selectedBuildingData = buildingData;
        selectedBuilding = Instantiate(buildingData.Prefab);
        selectedBuilding.SetGridCellSize(cellSize);
        selectedBuilding.PreviewValid();
        selectedBuilding.mapEditContext = mapEditionMode;


        isBuildingSelected = selectedBuilding != null;
        StartCoroutine(DelayedAddMouseEvents());
        Cursor.visible = false;
    }

    private void PlaceBuilding()
    {
        if (UIManager.Instance.IsMouseOverUI()) return;

        Vector3 worldMousePos = mouseControl.GetCursorMapPosition();
        Vector3Int gridMousePos = grid.WorldToCell(worldMousePos);

        bool canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, selectedBuildingData.Size);
        if (!canPlace)
            return;

        List<Vector3Int> occupiedPositions = BuildingGrid.AddObjectAt(
            gridMousePos,
            selectedBuildingData.Size,
            selectedBuildingData.IdNumber
        );

        if (buildingIsBeingMoved || mapEditionMode)
            selectedBuilding.Place();
        else
            selectedBuilding.StartConstruction(selectedBuildingData.ConstructionTime);

        selectedBuilding.SetID(selectedBuildingData.ID);
        selectedBuilding.occupiedGridPositions = occupiedPositions;
        selectedBuilding = null;
        isBuildingSelected = false;

        mouseControl.OnClicked -= PlaceBuilding;
        mouseControl.OnRightClicked -= CancelPlacement;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;

        mapGrid.gameObject.SetActive(false);
    }

    public void StartMoveBuilding(Building building)
    {
        selectedBuilding = building;
        isBuildingSelected = true;
        BuildingGrid.RemoveObjectAt(selectedBuilding.occupiedGridPositions);
        selectedBuilding.occupiedGridPositions.Clear();
        selectedBuilding.PreviewValid();
        StartCoroutine(DelayedAddMouseEvents());
        Cursor.visible = false;
    }

    public void PlaceBuildingAtLocation(Building building, Vector3 position, Vector2Int size)
    {
        var gridPos = grid.WorldToCell(position);
        BuildingGrid.AddObjectAt(gridPos, size, 0);
        building.transform.position = grid.CellToWorld(gridPos);
        building.StartConstruction(0);
        building.SetID(building.ID);
    }

    public void CancelPlacement()
    {
        if (isBuildingSelected)
        {
            Destroy(selectedBuilding.gameObject);
            selectedBuilding = null;
            selectedBuildingData = null;
            isBuildingSelected = false;
            mapGrid.gameObject.SetActive(false);
        }

        mouseControl.OnClicked -= PlaceBuilding;
        mouseControl.OnRightClicked -= CancelPlacement;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;
    }

    private IEnumerator DelayedAddMouseEvents()
    {
        yield return null;

        mouseControl.OnClicked += PlaceBuilding;
        mouseControl.OnRightClicked += CancelPlacement;
        mouseControl.OnExit += CancelPlacement;
    }
}