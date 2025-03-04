using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public LayerMask clickableLayer;

    private readonly List<BaseObject> selectedEntities = new();

    private BaseObject hoveredEntity;
    private bool isDragging;

    private Vector3 mouseDragStart;
    private bool selectionStarted;

    public static SelectionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (UIManager.Instance.IsMouseOverUI()) return;

        if (selectionStarted && (Input.mousePosition - mouseDragStart).magnitude > 5)
        {
            isDragging = true;
        }
        else
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayer))
            {
                var selectable = hit.collider.GetComponent<BaseObject>();
                if (selectable != null)
                    if (hoveredEntity != selectable)
                    {
                        hoveredEntity?.OnHoverExit();
                        hoveredEntity = selectable;
                        hoveredEntity.OnHoverEnter();
                    }
            }
            else if (!selectedEntities.Contains(hoveredEntity))
            {
                hoveredEntity?.OnHoverExit();
                hoveredEntity = null;
            }
        }
    }

    private void OnGUI()
    {
        if (isDragging)
        {
            var rect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 1, Color.blue);
        }
    }

    public void OnSelectStart()
    {
        if (UIManager.Instance.IsMouseOverUI()) return;

        selectionStarted = true;
        mouseDragStart = Input.mousePosition;
        isDragging = false;
    }

    public void OnSelectEnd()
    {
        if (UIManager.Instance.IsMouseOverUI()) return;

        if (isDragging)
        {
            SelectEntitiesInDrag();
            isDragging = false;
        }
        else
        {
            HandleSingleClick();
        }

        if (Input.GetMouseButton(0) && (Input.mousePosition - mouseDragStart).magnitude > 5) isDragging = true;
        selectionStarted = false;
    }

    private void HandleSingleClick()
    {
        if (UIManager.Instance.IsMouseOverUI()) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayer))
        {
            var selectable = hit.collider.GetComponent<BaseObject>();
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
        if (UIManager.Instance.IsMouseOverUI()) return;

        var selectionRect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
        var anySelected = false;
        foreach (var selectable in FindObjectsOfType<BaseObject>())
        {
            var screenPosition = Camera.main.WorldToScreenPoint(selectable.transform.position);
            screenPosition.y = Screen.height - screenPosition.y;
            if (selectionRect.Contains(screenPosition, true))
            {
                SelectEntity(selectable, true);
                anySelected = true;
            }
        }

        if (!anySelected && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) ClearSelection();
    }

    public void SelectEntity(BaseObject entity, bool isMultiSelect = false)
    {
        if (!isMultiSelect) ClearSelection();

        if (!entity.isSelected)
        {
            entity.Select();
            selectedEntities.Add(entity);
            UpdateUI();
        }
    }

    public void DeselectEntity(BaseObject entity)
    {
        if (entity != null && entity.isSelected)
        {
            entity.Deselect();
            selectedEntities.Remove(entity);
            UpdateUI();
        }
    }

    public void ClearSelection()
    {
        foreach (var entity in selectedEntities.ToList()) DeselectEntity(entity);
        selectedEntities.Clear();
        UpdateUI();
    }

    private List<BaseObject> GetSelectedProfiles()
    {
        return selectedEntities;
    }

    private void UpdateUI()
    {
        UIManager.Instance.UpdateSelectedEntities(GetSelectedProfiles());
        var entity = selectedEntities.FirstOrDefault() as Entity;
        if (entity != null)
            ActionsUIManager.Instance.UpdateActionButtons(entity.actionList);
        else
            ActionsUIManager.Instance.UpdateActionButtons(new List<EntityAction>());
    }

    public void OnInvokeActionable(int actionIndex)
    {
        if (selectedEntities.Count is 0 or > 1) return;
        var entity = selectedEntities[0] as Entity;
        entity?.actionList.ElementAtOrDefault(actionIndex).action.Invoke();
    }
}