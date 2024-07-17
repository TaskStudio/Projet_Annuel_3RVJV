using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance { get; private set; }

    public static List<IUnit> MovableUnits { get; } = new();

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
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
            {
                foreach (var entity in MovableUnits)
                    if (entity.IsSelected)
                        entity.SetTarget(hit.collider.GetComponent<IEntity>());
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                foreach (var entity in MovableUnits)
                    if (entity.IsSelected)
                    {
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            entity.MoveInFormation(hit.point);
                        else
                            entity.Move(hit.point);
                    }
            }
        }
    }

    public void RegisterMovableEntity(IUnit unit)
    {
        if (unit != null && !MovableUnits.Contains(unit)) MovableUnits.Add(unit);
    }

    public void UnregisterMovableEntity(IUnit unit)
    {
        if (unit != null) MovableUnits.Remove(unit);
    }

    public void UnregisterMovableEntity<T>(Unit<T> unit) where T : UnitData
    {
        UnregisterMovableEntity(unit as Unit);
    }
}