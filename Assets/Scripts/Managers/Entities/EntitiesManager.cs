using System.Collections.Generic;
using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public static EntitiesManager Instance { get; private set; }
    private static List<Unit> movableEntities = new List<Unit>();

    public static List<Unit> MovableEntities => movableEntities;

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
                foreach (var entity in movableEntities)
                {
                    if (entity is BaseObject selectable && selectable.IsSelected)
                    {
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        {
                            entity.MoveInFormation(hit.point);
                        }
                        else
                        {
                            entity.Move(hit.point);
                        }
                    }
                }
            }
        }
    }

    public void RegisterMovableEntity(Unit entity)
    {
        if (entity != null && !movableEntities.Contains(entity))
        {
            movableEntities.Add(entity);
        }
    }

    public void UnregisterMovableEntity(Unit entity)
    {
        if (entity != null)
        {
            movableEntities.Remove(entity);
        }
    }
}