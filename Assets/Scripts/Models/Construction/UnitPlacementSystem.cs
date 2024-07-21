using System.Collections;
using System.Collections.Generic;
using GameInput;
using UnityEngine;

public class UnitPlacementSystem : MonoBehaviour
{
    [SerializeField] private MouseControl mouseControl;
    [SerializeField] private LayerMask forbiddenLayers;
    [SerializeField] private UnitDatabaseSO unitDatase;

    [SerializeField] private List<EntityAction> unitsProductionOrders;

    private bool isUnitSelected;
    private Unit selectedUnit;
    private bool unitIsBeingMoved;

    private void Update()
    {
        if (isUnitSelected)
        {
            Vector3 worldMousePos = mouseControl.GetCursorMapPosition();

            if (Physics.Raycast(worldMousePos, Vector3.down, out RaycastHit _, Mathf.Infinity, forbiddenLayers))
            {
                selectedUnit.PreviewInvalid();
            }
            else
            {
                selectedUnit.PreviewValid();
                selectedUnit.transform.position = worldMousePos;
            }
        }
    }

    public void StartPlacement(string unitID)
    {
        CancelPlacement();

        selectedUnit = UnitFactory.SpawnEntity(unitID, new Vector3(0, 0, 0), unitDatase);
        selectedUnit.PreviewValid();

        isUnitSelected = selectedUnit != null;
        StartCoroutine(DelayedAddMouseEvents());
        Cursor.visible = false;
    }

    private void PlaceUnit()
    {
        if (UIManager.Instance.IsMouseOverUI()) return;

        selectedUnit.Place();
        selectedUnit = null;
        isUnitSelected = false;

        mouseControl.OnClicked -= PlaceUnit;
        mouseControl.OnRightClicked -= CancelPlacement;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;
    }

    public void StartMoveUnit(Unit unit)
    {
        selectedUnit = unit;
        isUnitSelected = true;
        selectedUnit.PreviewValid();
        StartCoroutine(DelayedAddMouseEvents());
        Cursor.visible = false;
    }

    public void CancelPlacement()
    {
        if (isUnitSelected)
        {
            Destroy(selectedUnit.gameObject);
            selectedUnit = null;
            isUnitSelected = false;
        }

        mouseControl.OnClicked -= PlaceUnit;
        mouseControl.OnRightClicked -= CancelPlacement;
        mouseControl.OnExit -= CancelPlacement;

        Cursor.visible = true;
    }

    private IEnumerator DelayedAddMouseEvents()
    {
        yield return null;

        mouseControl.OnClicked += PlaceUnit;
        mouseControl.OnRightClicked += CancelPlacement;
        mouseControl.OnExit += CancelPlacement;
    }
}