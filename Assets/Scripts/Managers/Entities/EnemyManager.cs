using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject suicideEnemyPrefab;
    public GameObject attackingEnemyPrefab;
    public GameObject defenderEnemyPrefab;
    public float spawnDelay = 2f;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        float spawnWidth = 10.0f;
        Vector3 spawnPosition = Vector3.zero;

        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            spawnPosition = transform.position + transform.right * randomX;
            spawnPosition.y = 1;

            if (!Physics.CheckSphere(spawnPosition, 0.5f))
            {
                int enemyType = Random.Range(0, 3); // Randomly choose enemy type
                switch (enemyType)
                {
                    case 0:
                        Instantiate(suicideEnemyPrefab, spawnPosition, Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(attackingEnemyPrefab, spawnPosition, Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(defenderEnemyPrefab, spawnPosition, Quaternion.identity);
                        break;
                }
                break;
            }
        }
    }
}