using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class EntitySpawner : MonoBehaviour {
    public GameObject entityPrefab;
    public GameObject spawnCenter; 
    public float spawnDelay = 2f;
    private Queue<int> spawnQueue = new Queue<int>();
    private bool isSpawning = false;
    public int entitiesPerRow = 5;
    public float spacing = 1f; 
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private Queue<Vector3> availablePositions = new Queue<Vector3>();

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
            yield return new WaitForSeconds(spawnDelay);
            FindAndSpawnEntity();
        }
        isSpawning = false;
    }

    void FindAndSpawnEntity() {
        Vector3 spawnPosition;
        if (availablePositions.Count > 0) {
            spawnPosition = availablePositions.Dequeue();
        } else {
            spawnPosition = CalculateNewPosition();
        }
        
        Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        spawnedPositions.Add(spawnPosition);
    }

    Vector3 CalculateNewPosition() {
        if (spawnedPositions.Count == 0) {
            return spawnCenter.transform.position;
        } else {
            NativeArray<Vector3> positions = new NativeArray<Vector3>(spawnedPositions.ToArray(), Allocator.TempJob);
            NativeArray<Vector3> result = new NativeArray<Vector3>(1, Allocator.TempJob);
            var spawnJob = new SpawnJob {
                positions = positions,
                entitiesPerRow = entitiesPerRow,
                spacing = spacing,
                spawnCenter = spawnCenter.transform.position,
                result = result
            };

            JobHandle jobHandle = spawnJob.Schedule();
            jobHandle.Complete();

            Vector3 spawnPosition = spawnJob.result[0];
            positions.Dispose();
            result.Dispose();

            return spawnPosition;
        }
    }

    public void FreePosition(Vector3 position) {
        spawnedPositions.Remove(position);
        availablePositions.Enqueue(position);
    }
}
