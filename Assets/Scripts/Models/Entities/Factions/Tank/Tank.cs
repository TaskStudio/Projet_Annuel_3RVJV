using System.Collections.Generic;
using UnityEngine;

public class Tank : Fighter
{
    private static readonly List<Tank> _selectedTanks = new();
    public GameObject combinedTankPrefab;
    public float tauntRadius = 10f;
    private bool isCombinedTank; // Flag to indicate if the tank is a combined tank

    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P)) CombineSelectedTanks();

        if (Input.GetKeyDown(KeyCode.O)) TauntEnemies();
    }

    public void CombineSelectedTanks()
    {
        // Add only selected normal tanks to the list
        if (IsSelected && !_selectedTanks.Contains(this) && !isCombinedTank) _selectedTanks.Add(this);

        // Ensure only normal tanks are in the selection
        if (_selectedTanks.Count == 10)
        {
            int combinedHp = 0;
            Vector3 combinedPosition = Vector3.zero;

            foreach (var tank in _selectedTanks)
            {
                combinedHp += tank.GetMaxHealthPoints();
                combinedPosition += tank.transform.position;
            }

            combinedPosition /= _selectedTanks.Count;
            combinedPosition.y = 2;

            foreach (var tank in _selectedTanks)
            {
                if (EntitiesManager.Instance) EntitiesManager.Instance.UnregisterMovableEntity(tank);
                Destroy(tank.gameObject);
            }

            GameObject newTankObject = Instantiate(combinedTankPrefab, combinedPosition, Quaternion.identity);
            newTankObject.transform.localScale = new Vector3(2, 2, 2);

            Tank newTank = newTankObject.GetComponent<Tank>();
            if (newTank)
            {
                newTank.currentHealth = combinedHp;
                newTank.SetMaxHealthPoints(combinedHp);

                newTank.movementSpeed = movementSpeed;
                newTank.stoppingDistance = stoppingDistance;
                newTank.entityLayer = entityLayer;
                newTank.collisionRadius = 2f;
                newTank.avoidanceStrength = avoidanceStrength;
                newTank.isCombinedTank = true; // Mark the new tank as a combined tank

                newTank.Start();
            }

            _selectedTanks.Clear();

            SelectionManager.Instance.ClearSelection();
            SelectionManager.Instance.SelectEntity(newTank);
        }
    }

    private void TauntEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, tauntRadius);
        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null) enemy.Taunt(this);
        }
    }
}