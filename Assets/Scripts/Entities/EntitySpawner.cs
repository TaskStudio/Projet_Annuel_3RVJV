using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class EntitySpawner : MonoBehaviour {
    public GameObject entityPrefab;
    public GameObject spawnCenter; 
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
            yield return new WaitForSeconds(spawnDelay);
            FindAndSpawnEntity();
        }
        isSpawning = false;
    }

    void FindAndSpawnEntity() {
        if (spawnedPositions.Count == 0) {
            Vector3 spawnPosition = spawnCenter.transform.position;
            Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition);
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
            Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition);

            positions.Dispose();
            result.Dispose();
        }
    }

    [BurstCompile]
    struct SpawnJob : IJob {
        [ReadOnly]
        public NativeArray<Vector3> positions;
        public int entitiesPerRow;
        public float spacing;
        public Vector3 spawnCenter;
        public NativeArray<Vector3> result;

        public void Execute() {
            for (int i = 0; ; i++) {
                int row = i / entitiesPerRow;
                int col = i % entitiesPerRow;
                Vector3 spawnPosition = CalculateSpawnPosition(col, row);

                bool isValid = true;
                for (int j = 0; j < positions.Length; j++) {
                    if (Vector3.Distance(positions[j], spawnPosition) < spacing) {
                        isValid = false;
                        break;
                    }
                }

                if (isValid) {
                    result[0] = spawnPosition;
                    break;
                }
            }
        }

        Vector3 CalculateSpawnPosition(int col, int row) {
            Vector3 offset = new Vector3(
                col * spacing - (entitiesPerRow - 1) * spacing / 2,
                0, 
                row * spacing - (entitiesPerRow - 1) * spacing / 2
            );

            return new Vector3(spawnCenter.x + offset.x, spawnCenter.y, spawnCenter.z + offset.z);
        }
    }
}
