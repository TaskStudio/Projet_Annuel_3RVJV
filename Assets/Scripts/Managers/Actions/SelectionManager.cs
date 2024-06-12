using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public LayerMask clickableLayer;

    private readonly List<ISelectable> selectedEntities = new();
    private bool isDragging;
    private Vector3 mouseDragStart;
    private bool selectionStarted;

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
        if (selectionStarted && (Input.mousePosition - mouseDragStart).magnitude > 5) isDragging = true;
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

    public void OnSelectStart()
    {
        selectionStarted = true;
        mouseDragStart = Input.mousePosition;
        isDragging = false;
    }

    public void OnSelectEnd()
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

        selectionStarted = false;
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

    private void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 1, Color.blue);
        }
    }

    private void SelectEntitiesInDrag()
    {
        Rect selectionRect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
        var anySelected = false;
        foreach (ISelectable selectable in FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>())
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(((MonoBehaviour)selectable).transform.position);
            screenPosition.y = Screen.height - screenPosition.y;
            if (selectionRect.Contains(screenPosition, true))
            {
                SelectEntity(selectable, true);
                anySelected = true;
            }
        }

        if (!anySelected && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) ClearSelection();
    }

    public void SelectEntity(ISelectable entity, bool isMultiSelect = false)
    {
        if (!isMultiSelect) ClearSelection();

        if (!entity.IsSelected)
        {
            entity.Select();
            selectedEntities.Add(entity);
            UpdateUI();
        }
    }

    public void DeselectEntity(ISelectable entity)
    {
        if (entity != null && entity.IsSelected)
        {
            entity.Deselect();
            selectedEntities.Remove(entity);
            UpdateUI();
        }
    }

    public void ClearSelection()
    {
        foreach (var entity in selectedEntities.ToList())
        {
            DeselectEntity(entity);
        }
        selectedEntities.Clear();
        UpdateUI();
    }

    public List<IProfile> GetSelectedProfiles()
    {
        return selectedEntities.Select(e => e.GetProfile()).ToList();
    }
    
    private void UpdateUI()
    {
        UIManager.Instance.UpdateSelectedEntities(GetSelectedProfiles());
    }
}