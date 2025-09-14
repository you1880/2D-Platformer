using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IBaseState
{
    private readonly EnemyStateMachine _enemyStateMachine;
    private const float ATTACK_DURATION = 1.0f;
    private float _attackTimer = 0.0f;

    public EnemyAttackState(EnemyStateMachine stateMachine)
    {
        _enemyStateMachine = stateMachine;
    }
    public void EnterState()
    {
    }

    public void UpdateState()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= ATTACK_DURATION)
        {
            _attackTimer = 0.0f;
            _enemyStateMachine.ChangeState(Define.EnemyState.Idle);
        }
    }

    public void ExitState()
    {
    }


}
