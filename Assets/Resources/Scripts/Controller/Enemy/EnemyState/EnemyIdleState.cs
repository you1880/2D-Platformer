using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IBaseState
{
    private readonly EnemyStateMachine _enemyStateMachine;
    private const float IDLE_TIME = 2.0f;
    private float _idleTimer = 0.0f;

    public EnemyIdleState(EnemyStateMachine stateMachine)
    {
        _enemyStateMachine = stateMachine;
    }

    public void EnterState()
    {
    }

    public void UpdateState()
    {
        _idleTimer += Time.deltaTime;
        if (_idleTimer >= IDLE_TIME)
        {
            _idleTimer = 0.0f;
            _enemyStateMachine.ChangeState(Define.EnemyState.Move);
        }
    }

    public void ExitState()
    {
        _idleTimer = 0.0f;
    }
}
