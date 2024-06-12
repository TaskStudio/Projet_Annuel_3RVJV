using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public LayerMask clickableLayer;
    public LayerMask groundLayer;

    public readonly List<ISelectable> selectedEntities = new();
    private bool isDragging;

    private Vector3 mouseDragStart;
    public static SelectionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDragStart = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                SelectEntitiesInDrag();
                isDragging = false;
            }
            else
            {
                HandleSingleClick();
            }
        }

        if (Input.GetMouseButton(0) && (Input.mousePosition - mouseDragStart).magnitude > 5) isDragging = true;
    }


    private void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 1, Color.blue);
        }
    }

    private void HandleSingleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayer))
        {
            var selectable = hit.collider.GetComponent<ISelectable>();
            if (selectable != null)
                SelectEntity(selectable, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            else
                ClearSelection();
        }
        else
        {
            ClearSelection();
        }
    }

    private void SelectEntitiesInDrag()
    {
        Rect selectionRect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
        bool anySelected = false;
        foreach (var selectable in FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>())
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(((MonoBehaviour) selectable).transform.position);
            screenPosition.y = Screen.height - screenPosition.y;
            if (selectionRect.Contains(screenPosition, true))
            {
                SelectEntity(selectable, true);
                anySelected = true;
            }
        }

        if (!anySelected && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) ClearSelection();
    }

    private void SelectEntity(ISelectable entity, bool isMultiSelect = false)
    {
        if (!isMultiSelect) ClearSelection();

        if (!entity.IsSelected)
        {
            entity.Select();
            selectedEntities.Add(entity);
        }
    }

    public void ClearSelection()
    {
        foreach (var entity in selectedEntities) entity.Deselect();
        selectedEntities.Clear();
    }

    public void OnInvokeActionable(int actionIndex)
    {
        if (selectedEntities.Count is 0 or > 1) return;
        var entity = selectedEntities[0] as Actionable;
        entity?.actionList[actionIndex].Invoke();
    }
}