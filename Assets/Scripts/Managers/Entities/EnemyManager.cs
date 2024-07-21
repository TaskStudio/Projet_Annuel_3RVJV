using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject suicideEnemyPrefab;
    public GameObject attackingEnemyPrefab;
    public GameObject enemyBossPrefab;
    public float spawnDelay = 2f;
    public Transform enemyBase;

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
            spawnPosition = enemyBase.position + enemyBase.right * randomX;
            spawnPosition.y = 0;
            
       

            // int enemyType = Random.Range(0, 3);
            // switch (enemyType)
            // {
            //     // case 0:
            //     //     Instantiate(suicideEnemyPrefab, spawnPosition, Quaternion.identity);
            //     //     break;
            //     // case 1:
            //     //     Instantiate(attackingEnemyPrefab, spawnPosition, Quaternion.identity);
            //     //     break;
            //     case 2:
            //         Instantiate(enemyBossPrefab, spawnPosition, Quaternion.identity);
            //         break;
            // }
        }
        Instantiate(enemyBossPrefab, spawnPosition, Quaternion.identity);
    }
}