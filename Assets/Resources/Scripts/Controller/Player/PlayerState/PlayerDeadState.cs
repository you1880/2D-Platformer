using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : IBaseState
{
    private PlayerStateMachine _playerStateMachine;
    private ISoundService _soundService;
    private const float RESPAWN_TIME = 1.5f;
    private float _timer = RESPAWN_TIME;
    public event Action OnCompleted;

    public PlayerDeadState(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
        _soundService = ServiceLocator.GetService<ISoundService>();
    }

    public void EnterState()
    {
        _timer = RESPAWN_TIME;
        _soundService.PlayEffect(Define.EffectSoundType.PlayerDead);
    }

    public void UpdateState()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0.0f)
        {
            OnCompleted?.Invoke();
        }
    }

    public void ExitState()
    {

    }
}
