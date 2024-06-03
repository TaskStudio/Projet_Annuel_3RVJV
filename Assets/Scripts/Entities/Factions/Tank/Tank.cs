using UnityEngine;
using System.Collections.Generic;

public class Tank : NonEnemy
{
    private static List<Tank> _selectedTanks = new List<Tank>();
    public GameObject combinedTankPrefab;
    public float tauntRadius = 10f;
    private bool isCombinedTank = false; // Flag to indicate if the tank is a combined tank

    protected new void Start()
    {
        base.Start();
        // Tanks do not shoot, so projectile related fields are set to null
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
        // Add only selected normal tanks to the list
        if (IsSelected && !_selectedTanks.Contains(this) && !isCombinedTank)
        {
            _selectedTanks.Add(this);
        }

        // Ensure only normal tanks are in the selection
        if (_selectedTanks.Count == 10)
        {
            int combinedHp = 0;
            Vector3 combinedPosition = Vector3.zero;

            foreach (var tank in _selectedTanks)
            {
                combinedHp += tank.hp;
                combinedPosition += tank.transform.position;
                Debug.Log($"Combining tank at position: {tank.transform.position} with HP: {tank.hp}");
            }

            combinedPosition /= _selectedTanks.Count;
            combinedPosition.y = 2;

            foreach (var tank in _selectedTanks)
            {
                if (EntitiesManager.Instance != null)
                {
                    EntitiesManager.Instance.UnregisterMovableEntity(tank);
                }
                Destroy(tank.gameObject);
            }

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
                newTank.collisionRadius = 2f;
                newTank.avoidanceStrength = this.avoidanceStrength;
                newTank.isCombinedTank = true; // Mark the new tank as a combined tank

                newTank.Start();
                Debug.Log($"Created new combined tank at position: {newTank.transform.position} with combined HP: {combinedHp}");
            }

            _selectedTanks.Clear();

            SelectionManager.Instance.ClearSelection();
            SelectionManager.Instance.SelectEntity(newTank, false);
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
