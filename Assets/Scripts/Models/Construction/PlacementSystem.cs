using System.Collections;
using FishNet;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using GameInput;

public class PlacementSystem : NetworkBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private MouseControl mouseControl;
    [SerializeField] private BuildingDatabaseSO buildingDatabase;
    public bool isBuildingSelected;

    public readonly BuildingGridData BuildingGrid = new();

    private Building selectedBuilding;
    private BuildingData selectedBuildingData;
    private int selectedBuildingID;

    private void Start()
    {
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
        if (selectedBuildingData != null)
        {
            selectedBuilding = Instantiate(selectedBuildingData.Prefab);
            selectedBuilding.gameObject.SetActive(true);
            selectedBuilding.PreviewValid();
            selectedBuildingID = ID;

            Debug.Log($"StartPlacement: Instantiated building prefab with ID {ID}, active state: {selectedBuilding.gameObject.activeSelf}");
            
            isBuildingSelected = selectedBuilding != null;
            StartCoroutine(DelayedAddMouseEvents());
        }
    }

    private void PlaceBuilding()
    {
        if (isBuildingSelected && selectedBuilding != null)
        {
            Vector3 worldMousePos = mouseControl.GetCursorMapPosition();
            Vector3Int gridMousePos = grid.WorldToCell(worldMousePos);

            bool canPlace = BuildingGrid.CanPlaceObjectAt(gridMousePos, selectedBuildingData.Size);
            if (!canPlace)
                return;

            // Add building data to the grid
            BuildingGrid.AddObjectAt(gridMousePos, selectedBuildingData.Size, selectedBuildingID, 0);

            // Command the server to spawn the building
            PlaceBuildingServerRpc(selectedBuildingID, gridMousePos, selectedBuilding.transform.rotation.eulerAngles);
            
            Debug.Log($"PlaceBuilding: Placement completed and server RPC called");
            
            selectedBuilding = null;
            isBuildingSelected = false;

            mouseControl.OnClicked -= PlaceBuilding;
            mouseControl.OnExit -= CancelPlacement;
        }
    }

    [ServerRpc]
    private void PlaceBuildingServerRpc(int buildingID, Vector3Int gridPosition, Vector3 rotation)
    {
        BuildingData buildingData = buildingDatabase.buildingsData.Find(x => x.IdNumber == buildingID);
        if (buildingData != null)
        {
            Vector3 worldPosition = grid.CellToWorld(gridPosition);
            Building buildingInstance = Instantiate(buildingData.Prefab, worldPosition, Quaternion.Euler(rotation));
            buildingInstance.gameObject.SetActive(true);
            NetworkObject networkObject = buildingInstance.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                Debug.Log($"PlaceBuildingServerRpc: Spawning building prefab with ID {buildingID}, active state: {buildingInstance.gameObject.activeSelf}");

                // Use the ServerManager to spawn the object over the network
                InstanceFinder.ServerManager.Spawn(networkObject);
                SetSpawnedObjectObserversRpc(networkObject);
            }
        }
    }

    [ObserversRpc]
    private void SetSpawnedObjectObserversRpc(NetworkObject networkObject)
    {
        if (networkObject != null && networkObject.gameObject != null)
        {
            Debug.Log($"SetSpawnedObjectObserversRpc: Building spawned and activated.");
            networkObject.gameObject.SetActive(true);
        }
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
