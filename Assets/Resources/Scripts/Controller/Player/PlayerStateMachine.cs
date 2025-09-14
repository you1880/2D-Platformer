using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    private readonly Dictionary<Define.PlayerState, IBaseState> _states = new();
    private IBaseState _currentState;
    public IBaseState CurrentState => _currentState;
    public event Action<Define.PlayerState> OnStateChanged;

    public PlayerStateMachine()
    {
        _states = new Dictionary<Define.PlayerState, IBaseState>()
        {
            {Define.PlayerState.Idle, new PlayerIdleState(this)},
            {Define.PlayerState.Appear, new PlayerAppearState(this)},
            {Define.PlayerState.Dead, new PlayerDeadState(this)},
            {Define.PlayerState.Hit, new PlayerHitState(this)},
        };

        ChangeState(Define.PlayerState.Appear);
    }

    public void ChangeState(Define.PlayerState newState)
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

    public bool CanInputState()
    {
        if (_currentState is PlayerAppearState ||
            _currentState is PlayerDeadState)
        {
            return false;
        }

        return true;
    }

    public IBaseState GetState(Define.PlayerState state)
    {
        if (_states.TryGetValue(state, out IBaseState baseState))
        {
            return baseState;
        }

        return null;
    }
}
