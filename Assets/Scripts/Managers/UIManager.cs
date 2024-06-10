using UnityEngine;
using System.Collections.Generic;

public class EntitiesManager : MonoBehaviour
{
    public static EntitiesManager Instance { get; private set; }
    private List<IMovable> movableEntities = new List<IMovable>();
    public UIManager uiManager; // Assurez-vous de l'assigner dans l'Inspector

    void Awake()
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
        if (Input.GetMouseButtonDown(1))  
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                MoveEntitiesInGrid(hit.point);
            }
        }

        // Update UI with selected entities
        UpdateSelectedEntitiesUI();
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

    private void MoveEntitiesInGrid(Vector3 targetPosition)
    {
        int entitiesPerSide = Mathf.CeilToInt(Mathf.Sqrt(movableEntities.Count));
        float spacing = 1f;
        float totalLength = spacing * (entitiesPerSide - 1);
        Vector3 startPoint = targetPosition - new Vector3(totalLength / 2, 0, totalLength / 2);

        int entityIndex = 0;
        foreach (var entity in movableEntities)
        {
            if (entity != null && entity is ISelectable selectable && selectable.IsSelected)
            {
                if ((entity as MonoBehaviour) != null) 
                {
                    int row = entityIndex / entitiesPerSide;
                    int column = entityIndex % entitiesPerSide;
                    Vector3 gridPosition = startPoint + new Vector3(spacing * column, 0, spacing * row);
                    entity.Move(gridPosition);
                    entityIndex++;
                }
                else
                {
                    UnregisterMovableEntity(entity); 
                }
            }
        }
    }

    private void UpdateSelectedEntitiesUI()
    {
        List<EntityProfile> selectedProfiles = new List<EntityProfile>();
        foreach (var entity in movableEntities)
        {
            if (entity is ISelectable selectable && selectable.IsSelected)
            {
                // Assuming each selectable entity has a way to provide its profile information
                var profile = new EntityProfile(selectable.Name, selectable.Image);
                selectedProfiles.Add(profile);
            }
        }
        uiManager.UpdateSelectedEntities(selectedProfiles);
    }
}