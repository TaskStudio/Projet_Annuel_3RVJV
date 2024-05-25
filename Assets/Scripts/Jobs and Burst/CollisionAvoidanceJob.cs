using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CollisionAvoidanceJob : IJobParallelFor
{
    public float avoidanceStrength;
    public float collisionRadius;
    public NativeArray<float3> positions;

    public void Execute(int index)
    {
        float3 position = positions[index];
        float3 avoidanceVector = float3.zero;

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == index) continue;

            float3 otherPosition = positions[i];
            float3 directionToOther = otherPosition - position;

            if (math.length(directionToOther) < collisionRadius)
            {
                avoidanceVector += math.normalize(position - otherPosition);
            }
        }

        if (math.any(math.isnan(position)) || math.any(math.isnan(avoidanceVector)))
        {
            Debug.LogError($"NaN detected in CollisionAvoidanceJob at index {index} - Position: {position}, AvoidanceVector: {avoidanceVector}");
        }

        if (!avoidanceVector.Equals(float3.zero))
        {
            positions[index] += math.normalize(avoidanceVector) * avoidanceStrength;
        }
    }
}