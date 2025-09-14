using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private Transform _transform;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _groundLayer;
    private IStageService _stageService;
    private IInputService _inputService;
    private IUserDataService _userDataService;
    private PlayerStateMachine _playerStateMachine;
    private PlayerAnimatior _playerAnimatior;
    private PlayerMovement _playerMovement;
    private bool _isInitialized = false;
    private bool _isDead = false;

    public void Hit(int damage)
    {
        if (_playerStateMachine.CurrentState is PlayerHitState || _playerStateMachine.CurrentState is PlayerDeadState || _playerStateMachine.CurrentState is PlayerAppearState)
        {
            return;
        }

        _userDataService.ReduceHp(damage);

        if (_userDataService.HP <= 0)
        {
            _playerStateMachine.ChangeState(Define.PlayerState.Dead);
            return;
        }

        _playerStateMachine.ChangeState(Define.PlayerState.Hit);
    }

    private void Init()
    {
        EnsureServices();
        _isInitialized = _inputService != null && _userDataService != null && _animator != null && _rigidbody != null;

        if (!_isInitialized) return;

        _playerStateMachine = new PlayerStateMachine();
        _playerAnimatior = new PlayerAnimatior(_animator);
        _playerMovement = new PlayerMovement(_collider2D, _rigidbody, _transform, _groundLayer);

        BindInputEvent();
        _playerStateMachine.ChangeState(Define.PlayerState.Appear);
    }

    private void OnKeyboard(Define.KeyboardEvent keyboardEvent)
    {
        if (_playerStateMachine.CanInputState() == false)
        {
            return;
        }
        float moveX = keyboardEvent switch
        {
            Define.KeyboardEvent.None => 0.0f,
            Define.KeyboardEvent.Left => -1.0f,
            Define.KeyboardEvent.Right => 1.0f,
            _ => 0.0f,
        };

        bool isRun = _inputService.IsShiftPressed;
        bool isJumpPressed = _inputService.IsSpacePressed;

        _playerMovement.SetMoveInfo(moveX, isRun, isJumpPressed);
    }

    private void OnStateChanged(Define.PlayerState newState)
    {
        _playerAnimatior.SetPlayerAnimation(newState);
    }

    private void OnDeath()
    {
        if (_isDead) return;
        
        _isDead = true;
        _stageService.FailStage(_stageService.CurrentStage);
    }

    private void BindInputEvent()
    {
        _inputService.KeyAction -= OnKeyboard;
        _inputService.KeyAction += OnKeyboard;

        _playerStateMachine.OnStateChanged -= OnStateChanged;
        _playerStateMachine.OnStateChanged += OnStateChanged;

        PlayerDeadState deadState = _playerStateMachine.GetState(Define.PlayerState.Dead) as PlayerDeadState;
        if (deadState != null)
        {
            deadState.OnCompleted -= OnDeath;
            deadState.OnCompleted += OnDeath;
        }
    }

    private void UnbindInputEvent()
    {
        _inputService.KeyAction -= OnKeyboard;
        _playerStateMachine.OnStateChanged -= OnStateChanged;

        PlayerDeadState deadState = _playerStateMachine.GetState(Define.PlayerState.Dead) as PlayerDeadState;
        if (deadState != null)
        {
            deadState.OnCompleted -= OnDeath;
        }
    }

    private void EnsureServices()
    {
        if (_inputService == null)
            _inputService = ServiceLocator.GetService<IInputService>();
        if (_userDataService == null)
            _userDataService = ServiceLocator.GetService<IUserDataService>();
        if (_stageService == null)
            _stageService = ServiceLocator.GetService<IStageService>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ServiceLocator.IsInitialized);

        Init();
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _playerStateMachine.UpdateState();
        _inputService.InputAction();
        _playerAnimatior.PlayMoveAnimation(_rigidbody.velocity);
        _playerMovement.DecreaseBufferTimer();
    }

    private void FixedUpdate()
    {
        if (!_isInitialized) return;

        _playerMovement.Move();
    }

    private void OnDiable()
    {
        UnbindInputEvent();
        _playerAnimatior?.Clear();
    }
}
