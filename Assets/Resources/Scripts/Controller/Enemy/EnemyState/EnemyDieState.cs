using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : IBaseState
{
    private readonly EnemyStateMachine _enemyStateMachine;
    private readonly BaseEnemy _self;
    private const float DIE_TIME = 1.0f;
    private float _dieTimer = DIE_TIME;

    public EnemyDieState(EnemyStateMachine stateMachine, BaseEnemy self)
    {
        _enemyStateMachine = stateMachine;
        _self = self;
    }

    public void EnterState() {}

    public void UpdateState()
    {
        _dieTimer -= Time.deltaTime;
        if (_dieTimer <= 0.0f)
        {
            _self.Death();
        }
    }

    public void ExitState() {}
}

