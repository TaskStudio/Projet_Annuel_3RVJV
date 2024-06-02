using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct MoveJob : IJob
{
    public Vector3 currentPosition;
    public Vector3 targetPosition;
    public float moveSpeed;
    public float deltaTime;
    public NativeArray<Vector3> newPosition;

    public void Execute()
    {
        Vector3 moveDirection = targetPosition - currentPosition;

        if (moveDirection == Vector3.zero)
        {
            moveDirection = new Vector3(0.01f, 0, 0.01f);
        }

        moveDirection.Normalize();

        Vector3 calculatedNewPosition = currentPosition + moveDirection * moveSpeed * deltaTime;
        calculatedNewPosition.y = currentPosition.y;

        newPosition[0] = calculatedNewPosition;
    }
}
