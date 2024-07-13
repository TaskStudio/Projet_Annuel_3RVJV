using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
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
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                break;
            }
        }
    }
}