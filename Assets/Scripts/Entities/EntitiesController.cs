using System.Collections.Generic;
using Construction;
using UnityEngine;

public class EntitiesController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask clickableLayer;

    [SerializeField] private PlacementSystem ps;
    private readonly List<GameObject> selectedEntities = new();
    private bool isDragging;
    private Vector3 mouseDragStart;

    private void Update()
    {
        if (ps.isBuildingSelected) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseDragStart = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, clickableLayer))
                SelectEntity(hit.collider.gameObject);
            else
                ClearSelection();
        }


        // Initiate Drag Selection
        if (Input.GetMouseButton(0))
            if ((mouseDragStart - Input.mousePosition).magnitude > 40)
                isDragging = true;

        // End Drag Selection
        if (Input.GetMouseButtonUp(0))
            if (isDragging)
            {
                SelectEntitiesInDrag();
                isDragging = false;
            }

        // Move Entities
        if (Input.GetMouseButtonDown(1) && selectedEntities.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                int entitiesPerSide = Mathf.CeilToInt(Mathf.Sqrt(selectedEntities.Count));
                var spacing = 1f; // Spacing
                float totalLength = spacing * (entitiesPerSide - 1);
                Vector3 startPoint = hit.point - new Vector3(totalLength / 2, 0, totalLength / 2);

                var entityIndex = 0;
                for (var i = 0; i < entitiesPerSide; i++)
                for (var j = 0; j < entitiesPerSide; j++)
                {
                    if (entityIndex >= selectedEntities.Count)
                        break;

                    Vector3 entityTargetPosition = startPoint + new Vector3(spacing * i, 1, spacing * j);

                    // Set the target position for the entity to move to
                    var entityMover = selectedEntities[entityIndex].GetComponent<EntityMover>();
                    if (entityMover != null) entityMover.SetTargetPosition(entityTargetPosition);

                    entityIndex++;
                }
            }
        }
    }


    private void OnGUI()
    {
        if (isDragging)
        {
            // Draw a GUI Box or rectangle as the selection box on screen
            Rect rect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 1, Color.blue);
        }
    }

    private void SelectEntity(GameObject entity, bool clearCurrentSelection = true)
    {
        if (clearCurrentSelection) ClearSelection();

        if (!selectedEntities.Contains(entity)) selectedEntities.Add(entity);
        entity.GetComponent<EntityVisuals>().UpdateVisuals(true);
    }


    private void ClearSelection()
    {
        for (int i = selectedEntities.Count - 1; i >= 0; i--)
        {
            GameObject entity = selectedEntities[i];
            if (entity == null)
            {
                selectedEntities.RemoveAt(i);
                continue;
            }

            var entityVisuals = entity.GetComponent<EntityVisuals>();
            if (entityVisuals != null) entityVisuals.UpdateVisuals(false);
        }

        selectedEntities.Clear();
    }


    private void SelectEntitiesInDrag()
    {
        Rect selectionRect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
        GameObject[] allEntities = GameObject.FindGameObjectsWithTag("Entity");

        foreach (GameObject entity in allEntities)
        {
            if (entity == null) continue;

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(entity.transform.position);
            screenPosition.y = Screen.height - screenPosition.y;

            if (selectionRect.Contains(screenPosition, true)) SelectEntity(entity, false);
        }
    }

    public void DeregisterEntity(GameObject entity)
    {
        selectedEntities.Remove(entity);
    }

    public bool IsEntitySelected(GameObject entity)
    {
        return selectedEntities.Contains(entity);
    }
}

// Utility class for drawing GUI elements
public static class Utils
{
    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
        Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Draw top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Draw left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Draw right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Draw bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}