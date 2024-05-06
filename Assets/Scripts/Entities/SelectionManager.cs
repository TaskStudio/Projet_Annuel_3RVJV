using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    private HashSet<GameObject> selectedEntities = new HashSet<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SelectEntity(GameObject entity, bool multiSelect = false)
    {
        if (!multiSelect)
        {
            DeselectAll();
        }

        selectedEntities.Add(entity);
        entity.GetComponent<EntityVisuals>().UpdateVisuals(true);
    }

    public void DeselectEntity(GameObject entity)
    {
        if (selectedEntities.Contains(entity))
        {
            selectedEntities.Remove(entity);
            entity.GetComponent<EntityVisuals>().UpdateVisuals(false);
        }
    }

    public void DeselectAll()
    {
        foreach (var entity in selectedEntities)
        {
            entity.GetComponent<EntityVisuals>().UpdateVisuals(false);
        }
        selectedEntities.Clear();
    }

    public bool IsEntitySelected(GameObject entity)
    {
        return selectedEntities.Contains(entity);
    }
    
    public List<GameObject> GetSelectedEntities()
    {
        return new List<GameObject>(selectedEntities);
    }
}