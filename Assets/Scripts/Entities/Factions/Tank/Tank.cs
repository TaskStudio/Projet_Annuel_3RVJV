using UnityEngine;
using System.Collections.Generic;

public class Tank : NonEnemy
{
    private static List<Tank> _selectedTanks = new List<Tank>();
    public GameObject combinedTankPrefab; // Reference to the combined tank prefab

    void Update()
    {
        base.Update(); // Ensure the base class Update method is called

        // Check for key press to combine selected tanks
        if (Input.GetKeyDown(KeyCode.P))
        {
            CombineSelectedTanks();
        }
        
    }

    

public void CombineSelectedTanks()
{
    // Ensure this tank is in the selected tanks list
    if (!_selectedTanks.Contains(this))
    {
        _selectedTanks.Add(this);
    }

    if (_selectedTanks.Count == 3)
    {
        Debug.Log("Combining three tanks...");

        // Calculate the combined HP and position
        int combinedHp = 0;
        Vector3 combinedPosition = Vector3.zero;

        foreach (var tank in _selectedTanks)
        {
            combinedHp += tank.hp;
            combinedPosition += tank.transform.position;
            Debug.Log("Combining tank with HP: " + tank.hp);
        }

        combinedPosition /= _selectedTanks.Count;
        combinedPosition.y = 2;

        // Destroy individual tanks
        foreach (var tank in _selectedTanks)
        {
            Debug.Log("Destroying tank with HP: " + tank.hp);
            Destroy(tank.gameObject);
        }

        // Create new combined tank from prefab
        GameObject newTankObject = Instantiate(combinedTankPrefab, combinedPosition, Quaternion.identity);
        newTankObject.transform.localScale = new Vector3(2, 2, 2); // Set the scale to 2, 2, 2

        Tank newTank = newTankObject.GetComponent<Tank>();
        if (newTank != null)
        {
            newTank.hp = combinedHp;

            // Optionally, set other properties or visuals of the new tank here
            newTank.projectilePrefab = this.projectilePrefab;
            newTank.projectileSpawnPoint = this.projectileSpawnPoint;
            newTank.selectionIndicatorPrefab = this.selectionIndicatorPrefab;
            newTank.moveSpeed = this.moveSpeed;
            newTank.stoppingDistance = this.stoppingDistance;
            newTank.Entity = this.Entity;
            newTank.collisionRadius = this.collisionRadius;
            newTank.avoidanceStrength = this.avoidanceStrength;

            Debug.Log("Created new combined tank with HP: " + newTank.hp + " at position: " + newTank.transform.position);

            // Ensure the new tank is initialized properly
            newTank.Start();
        }
        else
        {
            Debug.LogError("The combined tank prefab does not have a Tank component.");
        }

        // Clear the list of selected tanks
        _selectedTanks.Clear();

        // Select the new combined tank using SelectionManager
        SelectionManager.Instance.ClearSelection();
        SelectionManager.Instance.GetType().GetMethod("SelectEntity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(SelectionManager.Instance, new object[] { newTank, false });
    }
    else
    {
        Debug.Log("You must select exactly three tanks to combine. Selected tanks count: " + _selectedTanks.Count);
    }
}

}

