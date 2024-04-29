using GameInput;
using UnityEngine;

namespace Construction
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private MouseControl mouseControl;
        [SerializeField] private BuildingDatabaseSO buildingDatabase;
        public bool isBuildingSelected;

        private GridData buildingsData;

        private Building selectedBuilding;
        private BuildingData selectedBuildingData;
        private int selectedBuildingID;

        private void Start()
        {
            buildingsData = new GridData();
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

                bool canPlace = buildingsData.CanPlaceObjectAt(gridMousePos, buildingSize);
                if (!canPlace)
                    selectedBuilding.PreviewInvalid();
                else
                    selectedBuilding.PreviewValid();
            }
        }

        public void StartPlacement(int ID)
        {
            CancelPlacement();

            selectedBuildingData = buildingDatabase.buildingsData.Find(x => x.ID == ID);
            selectedBuilding = Instantiate(selectedBuildingData.Prefab);
            selectedBuilding.PreviewValid();
            selectedBuildingID = ID;

            isBuildingSelected = selectedBuilding != null;
            mouseControl.OnClicked += PlaceBuilding;
            mouseControl.OnExit += CancelPlacement;
        }

        private void PlaceBuilding()
        {
            // if (mouseControl.IsPointerOverUI())
            //     return;

            Vector3 worldMousePos = mouseControl.GetCursorMapPosition();
            Vector3Int gridMousePos = grid.WorldToCell(worldMousePos);

            bool canPlace = buildingsData.CanPlaceObjectAt(gridMousePos, selectedBuildingData.Size);
            if (!canPlace)
                return;

            buildingsData.AddObjectAt(gridMousePos, selectedBuildingData.Size, selectedBuildingID, 0);
            selectedBuilding.StartConstruction(selectedBuildingData.ConstructionTime);
            selectedBuilding = null;
            isBuildingSelected = false;

            mouseControl.OnClicked -= PlaceBuilding;
            mouseControl.OnExit -= CancelPlacement;
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
    }
}