using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected Define.EnemyType _enemyType = Define.EnemyType.None;
    [SerializeField] protected GameObject _headPoint;
    [SerializeField] protected BoxCollider2D _collider2D;
    [SerializeField] protected CircleCollider2D _sensorCollider2D;
    [SerializeField] protected Rigidbody2D _rigidbody2D;
    [SerializeField] protected Sensor _sensor;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected LayerMask _wallLayer;
    protected IDataService _dataService;
    protected IResourceService _resourceService;
    protected ISoundService _soundService;
    protected EnemyStateMachine _enemyStateMachine;
    protected EnemyMovement _enemyMovement;
    protected Data.Game.Enemy _enemyData;
    protected bool _isFacingRight = true;
    protected bool _isDead = false;
    protected int _maxHp;
    protected int _currentHp;
    protected int _attackDamage;
    protected float _moveSpeed;
    protected float _attackRange;
    protected float _attackDelay;
    protected float _attackDelayTimer = 0.0f;
    protected float _invincibleTimer = 0.0f;
    protected const float INVINCIBLE_TIME = 0.5f;
    protected abstract void Init();
    public abstract bool TakeDamage(int damage);
    public abstract void Attack();
    public virtual void Death()
    {
        if (_isDead)
            return;

        _resourceService.Destroy(gameObject, 1.0f);
    }

    protected void UpdateInvincibleTimer()
    {
        if (_invincibleTimer > 0.0f)
        {
            _invincibleTimer -= Time.deltaTime;
        }
    }

    protected void UpdateAttackDelayTimer()
    {
        if (_attackDelayTimer > 0.0f)
        {
            _attackDelayTimer -= Time.deltaTime;
        }
    }

    protected void SetSensorRange(float range)
    {
        if (_sensorCollider2D == null)
        {
            return;
        }

        _sensorCollider2D.radius = range;
    }

    private void DisableTriggers()
    {
        _headPoint.SetActive(false);
        _sensor.gameObject.SetActive(false);
    }

    private void OnStateChanged(Define.EnemyState newState)
    {
        if (newState == Define.EnemyState.Die)
        {
            DisableTriggers();
        }

        ChangeAnimation(newState.ToString());
    }

    private void ChangeAnimation(string animName)
    {
        _animator.CrossFade(animName, 0.1f);
    }

    private void Start()
    {
        _dataService = ServiceLocator.GetService<IDataService>();
        _resourceService = ServiceLocator.GetService<IResourceService>();
        _soundService = ServiceLocator.GetService<ISoundService>();
        _enemyStateMachine = new EnemyStateMachine(this);
        _enemyMovement = new EnemyMovement(this, _rigidbody2D);

        _enemyStateMachine.OnStateChanged += OnStateChanged;

        Init();
    }

    private void Update()
    {
        if (_attackDelayTimer <= 0.0f && _sensor.IsSensorOn)
        {
            Attack();
        }

        UpdateInvincibleTimer();
        UpdateAttackDelayTimer();

        _enemyStateMachine.UpdateState();
    }

    private void FixedUpdate()
    {
        if(_enemyStateMachine.CurrentState is EnemyMoveState)
            _enemyMovement.Move();
    }

    private void OnDisable()
    {
        _enemyStateMachine.OnStateChanged -= OnStateChanged;
    }
}
