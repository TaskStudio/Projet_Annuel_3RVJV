using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct SpawnJob : IJob
{
    [ReadOnly]
    public NativeArray<Vector3> positions;
    public int entitiesPerRow;
    public float spacing;
    public Vector3 spawnCenter;
    public NativeArray<Vector3> result;

    public void Execute()
    {
        for (int i = 0; ; i++)
        {
            int row = i / entitiesPerRow;
            int col = i % entitiesPerRow;
            Vector3 spawnPosition = CalculateSpawnPosition(col, row);

            bool isValid = true;
            for (int j = 0; j < positions.Length; j++)
            {
                if (Vector3.Distance(positions[j], spawnPosition) < spacing)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                result[0] = spawnPosition;
                break;
            }
        }
    }

    Vector3 CalculateSpawnPosition(int col, int row)
    {
        Vector3 offset = new Vector3(
            col * spacing - (entitiesPerRow - 1) * spacing / 2,
            0, 
            row * spacing - (entitiesPerRow - 1) * spacing / 2
        );

        return new Vector3(spawnCenter.x + offset.x, spawnCenter.y, spawnCenter.z + offset.z);
    }
}