using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public LayerMask clickableLayer; 
    public LayerMask groundLayer; 

    private Vector3 mouseDragStart;
    private bool isDragging;

    private readonly List<ISelectable> selectedEntities = new List<ISelectable>();

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

    void Update()
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

        if (Input.GetMouseButton(0) && (Input.mousePosition - mouseDragStart).magnitude > 5) 
        {
            isDragging = true;
        }
    }

    private void HandleSingleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayer)) // Only use clickableLayer
        {
            var selectable = hit.collider.GetComponent<ISelectable>();
            if (selectable != null)
            {
                SelectEntity(selectable, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            }
            else
            {
                ClearSelection();
            }
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
        bool anySelected = false;
        foreach (var selectable in FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>())
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(((MonoBehaviour)selectable).transform.position);
            screenPosition.y = Screen.height - screenPosition.y; // Convert to GUI coordinates
            if (selectionRect.Contains(screenPosition, true))
            {
                SelectEntity(selectable, true); // Multi-select is true
                anySelected = true;
            }
        }
        if (!anySelected && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            ClearSelection();
        }
    }

    private void SelectEntity(ISelectable entity, bool isMultiSelect = false)
    {
        if (!isMultiSelect)
        {
            ClearSelection();
        }

        if (!entity.IsSelected)
        {
            entity.Select();
            selectedEntities.Add(entity);
        }
    }

    public void ClearSelection()
    {
        foreach (var entity in selectedEntities)
        {
            entity.Deselect();
        }
        selectedEntities.Clear();
    }
}
