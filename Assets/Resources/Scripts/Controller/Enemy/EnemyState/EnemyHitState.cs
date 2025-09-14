using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : IBaseState
{
    private readonly EnemyStateMachine _enemyStateMachine;
    private const float HIT_TIME = 0.5f;
    private float _hitTimer = HIT_TIME;

    public EnemyHitState(EnemyStateMachine stateMachine)
    {
        _enemyStateMachine = stateMachine;
    }

    public void EnterState()
    {
        
    }
    public void UpdateState()
    {
        _hitTimer -= Time.deltaTime;
        if (_hitTimer <= 0.0f)
        {
            _enemyStateMachine.ChangeState(Define.EnemyState.Idle);
            _hitTimer = HIT_TIME;
        }
    }
    
    public void ExitState()
    {
    }
}
