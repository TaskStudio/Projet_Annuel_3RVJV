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
        float sqrCollisionRadius = collisionRadius * collisionRadius;

        for (int i = 0; i < unitPositions.Length; i++)
        {
            Vector3 neighborPosition = unitPositions[i];
            if (neighborPosition != currentPosition)
            {
                Vector3 collisionDirection = currentPosition - neighborPosition;
                float sqrDistance = collisionDirection.sqrMagnitude;

                if (sqrDistance < sqrCollisionRadius)
                {
                    float strength = avoidanceStrength / Mathf.Sqrt(sqrDistance);
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