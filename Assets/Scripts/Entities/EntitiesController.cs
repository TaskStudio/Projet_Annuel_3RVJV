using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask clickableLayer; // Set this to the layer your entities are on
    private List<GameObject> selectedEntities = new List<GameObject>();
    private bool isDragging = false;
    private Vector3 mouseDragStart;

    void Update()
    {
        // Single Selection
        if (Input.GetMouseButtonDown(0))
        {
            mouseDragStart = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, clickableLayer))
            {
                Debug.Log("Hit entity: " + hit.collider.gameObject.name); // Debug statement for entity selection
                SelectEntity(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("No entity hit"); // Debug statement for no hit
                ClearSelection();
            }
        }


        // Initiate Drag Selection
        if (Input.GetMouseButton(0))
        {
            if ((mouseDragStart - Input.mousePosition).magnitude > 40)
            {
                isDragging = true;
            }
        }

        // End Drag Selection
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                SelectEntitiesInDrag();
                isDragging = false;
            }
        }

        // Move Entities
        if (Input.GetMouseButtonDown(1) && selectedEntities.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Updated raycast without the layer mask and with a longer distance
            if (Physics.Raycast(ray, out hit, 1000)) // Increased distance
            {
                Debug.Log("Moving entity to: " + hit.point); // Check if this gets logged
                foreach (GameObject entity in selectedEntities)
                {
                    entity.transform.position = hit.point;
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any collider"); // Check if raycast is failing
            }
        }

    }

    private void SelectEntity(GameObject entity, bool clearCurrentSelection = true)
    {
        if (clearCurrentSelection)
        {
            ClearSelection();
        }

        if (!selectedEntities.Contains(entity))
        {
            selectedEntities.Add(entity);
        }
    }


    private void ClearSelection()
    {
        // Deselect all entities
        selectedEntities.Clear();
    }

    private void SelectEntitiesInDrag()
    {
        Rect selectionRect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
        GameObject[] allEntities = GameObject.FindGameObjectsWithTag("Entity"); // Ensure your entities have this tag

        Debug.Log("Selection box drawn: " + selectionRect); // Debug to check the rectangle size and position

        foreach (GameObject entity in allEntities)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(entity.transform.position);
            screenPosition.y = Screen.height - screenPosition.y; // Invert y for GUI coordinates

            if (selectionRect.Contains(screenPosition, true))
            {
                SelectEntity(entity, false); // false to not clear the current selection
                Debug.Log("Entity selected: " + entity.name); // Debug to check which entity is selected
            }
        }
    }



    void OnGUI()
    {
        if (isDragging)
        {
            // Draw a GUI Box or rectangle as the selection box on screen
            var rect = Utils.GetScreenRect(mouseDragStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 1, Color.blue);
        }
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
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
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
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Draw left
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Draw right
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Draw bottom
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
    
    
}
