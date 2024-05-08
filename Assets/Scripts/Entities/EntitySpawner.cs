using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour {
    public GameObject entityPrefab;
    public GameObject spawnCenter; // GameObject de référence pour le point de spawn
    public float spawnDelay = 2f;
    private Queue<int> spawnQueue = new Queue<int>();
    private bool isSpawning = false;
    public int entitiesPerRow = 5;
    public float spacing = 1f; 
    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            spawnQueue.Enqueue(1);
            if (!isSpawning) {
                StartCoroutine(ProcessSpawnQueue());
            }
        }
    }

    IEnumerator ProcessSpawnQueue() {
        isSpawning = true;
        while (spawnQueue.Count > 0) {
            spawnQueue.Dequeue();
            FindAndSpawnEntity();
            yield return new WaitForSeconds(spawnDelay);
        }
        isSpawning = false;
    }

    void FindAndSpawnEntity() {
        // Vérifie si aucune entité n'a été spawnée et si oui, spawn la première entité au spawnCenter
        if (spawnedPositions.Count == 0) {
            Vector3 spawnPosition = spawnCenter.transform.position;
            Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition);
        } else {
            for (int i = 0; ; i++) {
                int row = i / entitiesPerRow;
                int col = i % entitiesPerRow;
                Vector3 spawnPosition = CalculateSpawnPosition(col, row);
            
                if (!Physics.CheckSphere(spawnPosition, 0.5f)) {
                    Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
                    spawnedPositions.Add(spawnPosition);
                    break; 
                }
            }
        }
    }


    Vector3 CalculateSpawnPosition(int col, int row) {
        Vector3 basePosition = spawnCenter != null ? spawnCenter.transform.position : Vector3.zero;
        Vector3 offset = new Vector3(
            col * spacing - (entitiesPerRow - 1) * spacing / 2,
            0, 
            row * spacing - (entitiesPerRow - 1) * spacing / 2
        );

        return new Vector3(basePosition.x + offset.x, 1, basePosition.z + offset.z);
    }


}