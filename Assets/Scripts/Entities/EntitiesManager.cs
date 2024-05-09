using UnityEngine;
using System.Collections.Generic;

public class EntitiesManager : MonoBehaviour
{
    public static EntitiesManager Instance { get; private set; }
    private List<IMovable> movableEntities = new List<IMovable>();

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the manager across scenes
        }
    }

    void Update()
    {
        // Check for right mouse button click to initiate movement
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast to check if the ground is clicked
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                MoveEntities(hit.point);
            }
        }
    }

    public void RegisterMovableEntity(IMovable entity)
    {
        if (entity != null && !movableEntities.Contains(entity))
        {
            movableEntities.Add(entity);
        }
    }

    public void UnregisterMovableEntity(IMovable entity)
    {
        if (entity != null)
        {
            movableEntities.Remove(entity);
        }
    }

    private void MoveEntities(Vector3 targetPosition)
    {
        // Move all selected entities to the target position
        foreach (var entity in movableEntities)
        {
            if (entity is ISelectable selectable && selectable.IsSelected)
            {
                entity.Move(targetPosition);
            }
        }
    }
}