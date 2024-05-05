using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 2f;
    public float moveSpeed = 5f;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            GameObject enemy = SpawnEnemy();
            StartCoroutine(MoveEnemyToClosestEntity(enemy));
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private GameObject SpawnEnemy()
    {
        float spawnWidth = 10.0f; // La largeur de la ligne de spawn
        Vector3 spawnPosition = Vector3.zero; 
        bool positionFound = false;
        
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            spawnPosition = transform.position - transform.forward * 5 + transform.right * randomX;
            spawnPosition.y = 1;

            // Vérifie si l'espace est libre en utilisant la taille de l'ennemi
            if (!Physics.CheckSphere(spawnPosition, 0.5f))
            {
                positionFound = true;
                break;
            }
        }

        if (positionFound)
        {
            // Si une position valide est trouvée, fait apparaître l'ennemi
            return Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            // Si aucune position valide n'est trouvée, retourne null ou gère le cas autrement
            Debug.LogWarning("Couldn't find a valid spawn position for the enemy.");
            return null;
        }
    }

    


    private IEnumerator MoveEnemyToClosestEntity(GameObject enemy)
    {
        while (enemy != null)
        {
            Vector3 closestEntityPosition = FindClosestEntityPosition();
            
            if (!float.IsPositiveInfinity(closestEntityPosition.x))
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, closestEntityPosition, moveSpeed * Time.deltaTime);
            }
        
            yield return null;
        }
    }

    private Vector3 FindClosestEntityPosition()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        GameObject[] entityBases = GameObject.FindGameObjectsWithTag("EntityBase");
        Vector3 currentPosition = this.transform.position;

        // Find the closest entity or entity base
        GameObject closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        // Check entities first
        foreach (GameObject entity in entities)
        {
            Vector3 directionToTarget = entity.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = entity;
            }
        }

        // If no entity was found, check entity bases
        if (closestTarget == null && entityBases.Length > 0)
        {
            foreach (GameObject entityBase in entityBases)
            {
                Vector3 directionToTarget = entityBase.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = entityBase;
                }
            }
        }

        // Return the position of the closest target, or infinity if none found
        if (closestTarget != null)
        {
            return closestTarget.transform.position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

}
