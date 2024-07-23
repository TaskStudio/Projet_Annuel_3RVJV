using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance { get; private set; }

    public static List<Unit> MovableUnits { get; } = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetUnitsOrder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(
                ray,
                out hit,
                Mathf.Infinity,
                (1 << LayerMask.NameToLayer("Entity"))
                | (1 << LayerMask.NameToLayer("Enemy"))
            ))
        {
            foreach (var entity in MovableUnits)
                if (entity.IsSelected)
                    entity.SetTarget(hit.collider.GetComponent<Entity>());
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            foreach (var entity in MovableUnits)
                if (entity.IsSelected)
                {
                    if (entity is Enemy) return;
                    if (entity is AllyFighter allyFighter
                        && (Input.GetKey(KeyCode.LeftShift)
                            || Input.GetKey(KeyCode.RightShift)))
                        allyFighter.MoveAndAttack(hit.point);
                    else
                        entity.MoveInFormation(hit.point);
                }
        }
    }

    public void RegisterMovableEntity(Unit unit)
    {
        if (unit != null && !MovableUnits.Contains(unit)) MovableUnits.Add(unit);
    }

    public void UnregisterMovableEntity(Unit unit)
    {
        if (unit != null) MovableUnits.Remove(unit);
    }
}