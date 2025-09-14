using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    private readonly Dictionary<Define.EnemyState, IBaseState> _states = new();
    private BaseEnemy _self;
    private IBaseState _currentState;
    public IBaseState CurrentState => _currentState;
    public event Action<Define.EnemyState> OnStateChanged;

    public EnemyStateMachine(BaseEnemy self)
    {
        _self = self;
        _states = new Dictionary<Define.EnemyState, IBaseState>()
        {
            {Define.EnemyState.Idle, new EnemyIdleState(this)},
            {Define.EnemyState.Move, new EnemyMoveState(this)},
            {Define.EnemyState.Attack, new EnemyAttackState(this)},
            {Define.EnemyState.Hit, new EnemyHitState(this)},
            {Define.EnemyState.Die, new EnemyDieState(this, _self)}
        };

        ChangeState(Define.EnemyState.Idle);
    }

    public void ChangeState(Define.EnemyState newState)
    {
        if (_states.TryGetValue(newState, out IBaseState state) == false)
        {
            return;
        }

        if (_currentState != null)
            _currentState.ExitState();

        _currentState = state;
        _currentState.EnterState();

        OnStateChanged?.Invoke(newState);
    }

    public void UpdateState()
    {
        _currentState?.UpdateState();
    }

    private void Start()
    {
        
    }
}
