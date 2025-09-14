using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : IBaseState
{
    private PlayerStateMachine _playerStateMachine;
    private ISoundService _soundService;
    private const float HIT_DURATION = 1.5f;
    private float _hitTimer = HIT_DURATION;

    public PlayerHitState(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
        _soundService = ServiceLocator.GetService<ISoundService>();
    }

    public void EnterState()
    {
        _hitTimer = HIT_DURATION;
        _soundService.PlayEffect(Define.EffectSoundType.PlayerHit);
    }

    public void UpdateState()
    {
        _hitTimer -= Time.deltaTime;
        if (_hitTimer <= 0.0f)
        {
            _playerStateMachine.ChangeState(Define.PlayerState.Idle);
        }
    }

    public void ExitState()
    {
        _hitTimer = HIT_DURATION;
    }
}
