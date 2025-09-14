using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatior
{
    private bool _isStateAnimation = false;
    private string _currentAnimationName = "";
    private readonly Animator _playerAnimator;
    private const float ANIMATION_TRANS_DURATION = 0.05f;
    public Coroutine CurrentCoroutine { get; private set; }

    public PlayerAnimatior(Animator animator)
    {
        _playerAnimator = animator;
    }

    public void PlayMoveAnimation(Vector2 velocity)
    {
        if(_isStateAnimation)
        {
            return;
        }

        Define.PlayerState playerState = Define.PlayerState.Idle;

        if (velocity.y > 0.1f)
        {
            playerState = Define.PlayerState.Jump;
        }
        else if (Mathf.Abs(velocity.x) > 0.01f)
        {
            playerState = velocity.x > 0 ? Define.PlayerState.Walk : Define.PlayerState.Walk;
        }

        SetPlayerAnimation(playerState);
    }

    public void SetPlayerAnimation(Define.PlayerState playerState)
    {
        string animationName = GetAnimationName(playerState);
        if (string.IsNullOrEmpty(animationName))
        {
            return;
        }

        IsStateAnimation(playerState);
        PlayAnimation(animationName);

        if (_isStateAnimation)
        {
            CurrentCoroutine = CoroutineHandler.Instance.RunCoroutine(EnsureAnimationFinished(animationName));
        }
    }

    private void PlayAnimation(string animName)
    {
        if (_currentAnimationName == animName)
        {
            return;
        }

        _playerAnimator.CrossFade(animName, ANIMATION_TRANS_DURATION);
        _currentAnimationName = animName;
    }

    private void IsStateAnimation(Define.PlayerState playerState)
    {
        _isStateAnimation = playerState == Define.PlayerState.Appear
            || playerState == Define.PlayerState.Dead
            || playerState == Define.PlayerState.Hit;
    }

    private string GetAnimationName(Define.PlayerState playerState)
        => Enum.GetName(typeof(Define.PlayerState), playerState);

    private IEnumerator EnsureAnimationFinished(string animatioName)
    {
        while (_playerAnimator != null) 
        {
            var currentState = _playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentState.normalizedTime >= 1.0f && !currentState.IsName(animatioName))
            {
                _isStateAnimation = false;
                break;
            }
            
            yield return null;
        }

        _isStateAnimation = false;
    }

    public void Clear()
    {
        if (CurrentCoroutine != null)
        {
            CoroutineHandler.Instance.StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
        }
        _isStateAnimation = false;
    }
}
