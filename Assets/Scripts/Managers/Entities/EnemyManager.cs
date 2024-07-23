using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject suicideEnemyPrefab;
    public GameObject attackingEnemyPrefab;
    public GameObject enemyBossPrefab;
    public float initialDelay = 300f; 
    public float enemySpawnInterval = 120f; 
    public float bossSpawnTime = 1800f; 
    public Transform enemyBase;

    private void Start()
    {
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(BossSpawnRoutine());
    }

    private IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay); 

        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    private IEnumerator BossSpawnRoutine()
    {
        yield return new WaitForSeconds(bossSpawnTime); 
        SpawnBoss();
    }

    private void SpawnEnemy()
    {
        float spawnWidth = 10.0f;
        Vector3 spawnPosition = Vector3.zero;

        float randomX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
        spawnPosition = enemyBase.position + enemyBase.right * randomX;
        spawnPosition.y = 1;

        int enemyType = Random.Range(0, 2); 
        switch (enemyType)
        {
            case 0:
                Instantiate(suicideEnemyPrefab, spawnPosition, Quaternion.identity);
                break;
            case 1:
                Instantiate(attackingEnemyPrefab, spawnPosition, Quaternion.identity);
                break;
        }
    }

    private void SpawnBoss()
    {
        float spawnWidth = 10.0f;
        float randomX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
        Vector3 spawnPosition = enemyBase.position + enemyBase.right * randomX;
        spawnPosition.y = 0;

        Instantiate(enemyBossPrefab, spawnPosition, Quaternion.identity);
    }
}
