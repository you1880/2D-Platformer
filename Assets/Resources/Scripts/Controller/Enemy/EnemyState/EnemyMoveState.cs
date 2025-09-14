using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : IBaseState
{
    private readonly EnemyStateMachine _enemyStateMachine;
    private const float MOVE_DURATION = 2.0f;
    private float _moveTimer = 0.0f;

    public EnemyMoveState(EnemyStateMachine stateMachine)
    {
        _enemyStateMachine = stateMachine;
    }
    public void EnterState()
    {
    }

    public void UpdateState()
    {
        _moveTimer += Time.deltaTime;
        if (_moveTimer >= MOVE_DURATION)
        {
            _moveTimer = 0.0f;
            _enemyStateMachine.ChangeState(Define.EnemyState.Idle);
        }
    }

    public void ExitState()
    {
        _moveTimer = 0.0f;
    }
}
