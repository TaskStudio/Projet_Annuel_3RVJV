using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct PlayerAttackJob : IJob
{
    public int Damage;

    public void Execute()
    {
    }
}