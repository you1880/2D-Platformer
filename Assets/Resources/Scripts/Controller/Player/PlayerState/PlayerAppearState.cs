using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearState : IBaseState
{
    private readonly PlayerStateMachine _playerStateMachine;
    private float _appearTimer;
    private const float DURATION = 1.0f;

    public PlayerAppearState(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
    }

    public void EnterState()
    {
        _appearTimer = 0.0f;
    }

    public void ExitState() {}

    public void UpdateState()
    {
        _appearTimer += Time.deltaTime;

        if (_appearTimer >= DURATION)
        {
            _playerStateMachine.ChangeState(Define.PlayerState.Idle);
        }
    }
}
