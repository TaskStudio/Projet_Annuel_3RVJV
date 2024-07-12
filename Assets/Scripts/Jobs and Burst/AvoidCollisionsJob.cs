using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct AvoidCollisionsJob : IJob
{
    [ReadOnly] public NativeArray<Vector3> unitPositions;
    [ReadOnly] public Vector3 currentPosition;
    public float collisionRadius;
    public float avoidanceStrength;
    public NativeArray<Vector3> avoidanceVector;

    public void Execute()
    {
        Vector3 avoidance = Vector3.zero;

        for (int i = 0; i < unitPositions.Length; i++)
        {
            Vector3 neighborPosition = unitPositions[i];
            if (neighborPosition != currentPosition)
            {
                Vector3 collisionDirection = currentPosition - neighborPosition;
                float distance = collisionDirection.magnitude;

                if (distance < collisionRadius)
                {
                    float strength = avoidanceStrength * (1 / distance);
                    avoidance += collisionDirection.normalized * strength;
                }
            }
        }

        if (avoidance != Vector3.zero)
        {
            avoidance = avoidance.normalized * avoidanceStrength;
        }

        avoidanceVector[0] = avoidance;
    }
}