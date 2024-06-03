using UnityEngine;
using System.Collections.Generic;

public class Tank : NonEnemy
{
    private static List<Tank> _selectedTanks = new List<Tank>();
    public GameObject combinedTankPrefab;
    public float tauntRadius = 10f; 

    protected new void Start()
    {
        base.Start();
        
        projectilePrefab = null;
        projectileSpawnPoint = null;
    }

    protected new void Update()
    {
        base.Update(); 
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            CombineSelectedTanks();
        }

        // Check for key press to taunt nearby enemies
        if (Input.GetKeyDown(KeyCode.O))
        {
            TauntEnemies();
        }
    }

    public override void Shoot(Vector3 target)
    {
        // Tanks do not shoot, so this method does nothing
    }

    public void CombineSelectedTanks()
    {
        if (!_selectedTanks.Contains(this))
        {
            _selectedTanks.Add(this);
        }

        if (_selectedTanks.Count == 10)
        {
            int combinedHp = 0;
            Vector3 combinedPosition = Vector3.zero;

            foreach (var tank in _selectedTanks)
            {
                combinedHp += tank.hp;
                combinedPosition += tank.transform.position;
            }

            combinedPosition /= _selectedTanks.Count;
            combinedPosition.y = 2;

            // Unregister and destroy individual tanks
            foreach (var tank in _selectedTanks)
            {
                EntitiesManager.Instance.UnregisterMovableEntity(tank);
                Destroy(tank.gameObject);
            }

            // Create new combined tank from prefab
            GameObject newTankObject = Instantiate(combinedTankPrefab, combinedPosition, Quaternion.identity);
            newTankObject.transform.localScale = new Vector3(2, 2, 2);

            Tank newTank = newTankObject.GetComponent<Tank>();
            if (newTank != null)
            {
                newTank.hp = combinedHp;
                newTank.selectionIndicatorPrefab = this.selectionIndicatorPrefab;
                newTank.moveSpeed = this.moveSpeed;
                newTank.stoppingDistance = this.stoppingDistance;
                newTank.Entity = this.Entity;
                newTank.collisionRadius = this.collisionRadius;
                newTank.avoidanceStrength = this.avoidanceStrength;
                
                
                newTank.Start();
            }

            // Clear the list of selected tanks
            _selectedTanks.Clear();

            // Select the new combined tank using SelectionManager
            SelectionManager.Instance.ClearSelection();
            SelectionManager.Instance.GetType().GetMethod("SelectEntity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .Invoke(SelectionManager.Instance, new object[] { newTank, false });
        }
    }

    private void TauntEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, tauntRadius);
        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Taunt(this);
            }
        }
    }
}
