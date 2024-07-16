using System.Collections;
using GameInput;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private MapGrid mapGrid;
    [SerializeField] private MouseControl mouseControl;
    [SerializeField] private BuildingDatabaseSO buildingDatabase;
    [SerializeField] private float cellSize = 1.0f;

    public bool isBuildingSelected;

    private readonly BuildingGridData BuildingGrid = new();
    private Grid grid;

    private Building selectedBuilding;
    private BuildingData selectedBuildingData;
    private int selectedBuildingID;

    private void Start()
    {
        grid = mapGrid.Grid;
        mapGrid.SetGridCellSize(cellSize);

        isBuildingSelected = false;

        selectedBuilding = null;
        selectedBuildingData = null;
        selectedBuildingID = -1;
    }

    private void Update()
    {
        if (isBuildingSelected && selectedBuilding.state == Building.BuildingStates.Preview)
        {
            Vector3 worldMousePos = mouseControl.GetCursorMapPosition();
            Vector3Int gridMousePos = grid.WorldToCell(worldMousePos);
            Vector3 gridWorldPos = grid.CellToWorld(gridMousePos);
            selectedBuilding.transform.position = gridWorldPos;

            Vector2Int buildingSize = selectedBuildingData.Size;

            bool canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, buildingSize);
            if (!canPlace)
                selectedBuilding.PreviewInvalid();
            else
                selectedBuilding.PreviewValid();
        }
    }

    public void StartPlacement(int ID)
    {
        CancelPlacement();

        selectedBuildingData = buildingDatabase.buildingsData.Find(x => x.IdNumber == ID);
        selectedBuilding = Instantiate(selectedBuildingData.Prefab);
        selectedBuilding.SetGridCellSize(cellSize);
        selectedBuilding.PreviewValid();
        selectedBuildingID = ID;

        isBuildingSelected = selectedBuilding != null;
        StartCoroutine(DelayedAddMouseEvents());
        Cursor.visible = false;
    }

    private void PlaceBuilding()
    {
        // if (mouseControl.IsPointerOverUI())
        //     return;

        Vector3 worldMousePos = mouseControl.GetCursorMapPosition();
        Vector3Int gridMousePos = grid.WorldToCell(worldMousePos);

        bool canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, selectedBuildingData.Size);
        if (!canPlace)
            return;

        BuildingGrid.AddObjectAt(gridMousePos, selectedBuildingData.Size, selectedBuildingID, 0);
        selectedBuilding.StartConstruction(selectedBuildingData.ConstructionTime);
        selectedBuilding = null;
        isBuildingSelected = false;

        mouseControl.OnClicked -= PlaceBuilding;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;
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
        // Attendre la fin de la frame actuelle
        yield return null;

        mouseControl.OnClicked += PlaceBuilding;
        mouseControl.OnExit += CancelPlacement;
    }
}