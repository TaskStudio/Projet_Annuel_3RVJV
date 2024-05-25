using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct MovementJob : IJobParallelFor
{
    public float deltaTime;
    public float moveSpeed;
    public NativeArray<float3> positions;
    public NativeArray<float3> targets;

    public void Execute(int index)
    {
        float3 position = positions[index];
        float3 target = targets[index];
        float3 direction = math.normalize(target - position);

        if (math.any(math.isnan(position)) || math.any(math.isnan(target)) || math.any(math.isnan(direction)))
        {
            Debug.LogError($"NaN detected in MovementJob at index {index} - Position: {position}, Target: {target}, Direction: {direction}");
        }

        positions[index] = position + direction * moveSpeed * deltaTime;
    }
}