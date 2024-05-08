using UnityEngine;
using System.Collections.Generic;

public class EntitiesManager : MonoBehaviour
{
    public static EntitiesManager Instance { get; private set; }
    private List<IMovable> movableEntities = new List<IMovable>();

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                MoveEntities(hit.point);
            }
        }
    }

    public void RegisterMovableEntity(IMovable entity)
    {
        if (!movableEntities.Contains(entity))
        {
            movableEntities.Add(entity);
        }
    }

    public void UnregisterMovableEntity(IMovable entity)
    {
        movableEntities.Remove(entity);
    }

    public void MoveEntities(Vector3 targetPosition)
    {
        foreach (var entity in movableEntities)
        {
            if (entity is ISelectable selectable && selectable.IsSelected)
            {
                entity.Move(targetPosition);
            }
        }
    }
}