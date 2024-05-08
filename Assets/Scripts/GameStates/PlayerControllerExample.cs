using UnityEngine;
using Shared;
using Unity.Jobs;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine;
    public Vector3 Direction = Vector3.forward;
    public float MoveSpeed = 5f;
    public int AttackDamage = 10;

    void Awake()
    {
        StateMachine = new PlayerStateMachine();
    }

    void Update()
    {
        switch (StateMachine.CurrentState)
        {
            case PlayerState.Moving:
                ExecuteMoveJob();
                break;
            case PlayerState.Attacking:
                ExecuteAttackJob();
                break;
            default:
                break;
        }
    }

    private void ExecuteMoveJob()
    {
        var moveJob = new PlayerMoveJob
        {
            Position = transform.position,
            Direction = Direction,
            Speed = MoveSpeed,
            DeltaTime = Time.deltaTime
        };

        var handle = moveJob.Schedule();
        handle.Complete();
        transform.position = moveJob.Position;
    }

    private void ExecuteAttackJob()
    {
        var attackJob = new PlayerAttackJob
        {
            Damage = AttackDamage
        };

        var handle = attackJob.Schedule();
        handle.Complete();
    }

    public void ChangeState(PlayerState newState)
    {
        StateMachine.ChangeState(newState);
    }
}