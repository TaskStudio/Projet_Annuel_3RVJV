using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject entityPrefab;
    public GameObject spawnCenter;
    public float spawnDelay = 2f;
    public int entitiesPerRow = 5;
    public float spacing = 1f;
    private bool isSpawning;
    private readonly List<Vector3> spawnedPositions = new();
    private readonly Queue<int> spawnQueue = new();


    private IEnumerator ProcessSpawnQueue()
    {
        isSpawning = true;
        while (spawnQueue.Count > 0)
        {
            spawnQueue.Dequeue();
            yield return new WaitForSeconds(spawnDelay);
            FindAndSpawnEntity();
        }

        isSpawning = false;
    }

    private void FindAndSpawnEntity()
    {
        Vector3 spawnPosition;
        if (spawnedPositions.Count == 0)
        {
            spawnPosition = spawnCenter.transform.position;
        }
        else
        {
            NativeArray<Vector3> positions = new(spawnedPositions.ToArray(), Allocator.TempJob);
            NativeArray<Vector3> result = new(1, Allocator.TempJob);
            var spawnJob = new SpawnJob
            {
                positions = positions,
                entitiesPerRow = entitiesPerRow,
                spacing = spacing,
                spawnCenter = spawnCenter.transform.position,
                result = result
            };

            JobHandle jobHandle = spawnJob.Schedule();
            jobHandle.Complete();

            spawnPosition = spawnJob.result[0];
            positions.Dispose();
            result.Dispose();
        }

        Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        spawnedPositions.Add(spawnPosition);
    }

    public void FreePosition(Vector3 position)
    {
        if (this != null) spawnedPositions.Remove(position);
    }
}