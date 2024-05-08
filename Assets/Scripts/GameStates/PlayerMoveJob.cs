using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct PlayerMoveJob : IJob
{
    public Vector3 Position;
    public Vector3 Direction;
    public float Speed;
    public float DeltaTime;

    public void Execute()
    {
        Position += Direction * Speed * DeltaTime;
    }
}