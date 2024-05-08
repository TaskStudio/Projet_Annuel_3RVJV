using UnityEngine;
using Shared;

public class PlayerStateLogicExample : MonoBehaviour
{
    private PlayerController _controller;

    void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _controller.ChangeState(PlayerState.Moving);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            _controller.ChangeState(PlayerState.Attacking);
        }
        else
        {
            _controller.ChangeState(PlayerState.Idle);
        }
    }
}