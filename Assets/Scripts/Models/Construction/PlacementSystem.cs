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

    public static PlacementSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mapGrid.SetGridCellSize(cellSize);

        isBuildingSelected = false;

        selectedBuilding = null;
        selectedBuildingData = null;
    }

    private void Update()
    {
        if (isBuildingSelected && selectedBuilding.state == Building.BuildingStates.Preview)
        {
            var worldMousePos = mouseControl.GetCursorMapPosition();
            var gridMousePos = grid.WorldToCell(worldMousePos);
            var gridWorldPos = grid.CellToWorld(gridMousePos);
            selectedBuilding.transform.position = gridWorldPos;

            var buildingSize = selectedBuildingData.Size;

            var canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, buildingSize);
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

        BuildingGrid.AddObjectAt(gridMousePos, selectedBuildingData.Size, 0);
        selectedBuilding.StartConstruction(selectedBuildingData.ConstructionTime);
        selectedBuilding.SetID(selectedBuildingData.ID);
        selectedBuilding.Size = selectedBuildingData.Size;
        selectedBuilding = null;
        isBuildingSelected = false;

        mouseControl.OnClicked -= PlaceBuilding;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;
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
        }
    }

    private IEnumerator DelayedAddMouseEvents()
    {
        yield return null;

        mouseControl.OnClicked += PlaceBuilding;
        mouseControl.OnExit += CancelPlacement;
    }
}