using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask clickableLayer;
    public float entitySpacing = 1f;

    private void Update()
    {
        HandleSelectionInput();
        HandleMovementInput();
    }

    private void HandleSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, clickableLayer))
            {
                Debug.Log("Hit: " + hit.collider.name); // This will log the name of the hit object
                bool isMultiSelect = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                SelectionManager.Instance.SelectEntity(hit.collider.gameObject, isMultiSelect);
            }
            else
            {
                Debug.Log("No hit detected"); // Log if no hit was detected
                if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                {
                    SelectionManager.Instance.DeselectAll();
                }
            }
        }
    }


    private void HandleMovementInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MoveSelectedEntities();
        }
    }

    private void MoveSelectedEntities()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            foreach (var entity in SelectionManager.Instance.GetSelectedEntities())
            {
                IMovable movableComponent = entity.GetComponent<IMovable>();
                if (movableComponent != null)
                {
                    movableComponent.Move(CalculatePositionNear(hit.point, entity));
                }
            }
        }
    }

    private Vector3 CalculatePositionNear(Vector3 hitPoint, GameObject entity)
    {
        int index = SelectionManager.Instance.GetSelectedEntities().IndexOf(entity);
        int row = index / 5; 
        int col = index % 5;
        return new Vector3(hitPoint.x + col * entitySpacing, hitPoint.y, hitPoint.z + row * entitySpacing);
    }
}
