using System.Collections;
using GameInput;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private MapGrid mapGrid;
    [SerializeField] private MouseControl mouseControl;
    [SerializeField] private float cellSize = 1.0f;

    public bool isBuildingSelected;

    private readonly BuildingGridData BuildingGrid = new();
    private Grid grid;

    private Building selectedBuilding;
    private BuildingData selectedBuildingData;

    private void Start()
    {
        grid = mapGrid.Grid;
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

    public void StartPlacement(BuildingData buildingData)
    {
        CancelPlacement();
        mapGrid.gameObject.SetActive(true);

        selectedBuildingData = buildingData;
        selectedBuilding = Instantiate(buildingData.Prefab);
        selectedBuilding.SetGridCellSize(cellSize);
        selectedBuilding.PreviewValid();

        isBuildingSelected = selectedBuilding != null;
        StartCoroutine(DelayedAddMouseEvents());
        Cursor.visible = false;
    }

    private void PlaceBuilding()
    {
        var worldMousePos = mouseControl.GetCursorMapPosition();
        var gridMousePos = grid.WorldToCell(worldMousePos);

        var canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, selectedBuildingData.Size);
        if (!canPlace)
            return;

        BuildingGrid.AddObjectAt(gridMousePos, selectedBuildingData.Size, selectedBuildingData.IdNumber, 0);
        selectedBuilding.StartConstruction(selectedBuildingData.ConstructionTime);
        selectedBuilding = null;
        isBuildingSelected = false;

        mouseControl.OnClicked -= PlaceBuilding;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;

        mapGrid.gameObject.SetActive(false);
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
    }

    private IEnumerator DelayedAddMouseEvents()
    {
        yield return null;

        mouseControl.OnClicked += PlaceBuilding;
        mouseControl.OnExit += CancelPlacement;
    }
}