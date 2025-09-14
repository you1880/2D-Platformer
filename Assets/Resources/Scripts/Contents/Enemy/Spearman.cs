using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spearman : BaseEnemy
{
    [SerializeField] private GameObject _spear;
    private const float SPEAR_SPEED = 1.8f;

    public override bool TakeDamage(int damage)
    {
        if (_invincibleTimer > 0.0f || _enemyStateMachine.CurrentState is EnemyHitState || _enemyStateMachine.CurrentState is EnemyDieState)
            return false;

        Debug.Log($"Spearman TakeDamage: {damage}");
        _currentHp -= damage;
        _invincibleTimer = INVINCIBLE_TIME;

        if (_currentHp <= 0)
        {
            _enemyStateMachine.ChangeState(Define.EnemyState.Die);

        }
        else
        {
            _enemyStateMachine.ChangeState(Define.EnemyState.Hit);
        }

        return true;
    }

    public override void Attack()
    {
        Transform target = _sensor.DetectionTarget.transform;
        if (target == null)
        {
            return;
        }

        _soundService.PlayEffect(Define.EffectSoundType.SpearmanAttack);
        _enemyMovement.ChangeAttackDirection(target, transform);
        _enemyStateMachine.ChangeState(Define.EnemyState.Attack);
        _attackDelayTimer = _attackDelay;

        ThrowSpear(target.position);
    }

    protected override void Init()
    {
        _enemyType = Define.EnemyType.Spearman;
        _enemyData = _dataService.GetEnemy(_enemyType);

        if (_enemyData != null)
        {
            _maxHp = _currentHp = _enemyData.maxHp;
            _attackDamage = _enemyData.attack;
            _attackRange = _enemyData.attackRange;
            _moveSpeed = _enemyData.moveSpeed;
            _attackDelay = _enemyData.attackDelay;
            _attackDelayTimer = 0.0f;
            _invincibleTimer = 0.0f;
        }

        SetSensorRange(_attackRange);
    }

    private void ThrowSpear(Vector3 target)
    {
        if (_spear == null)
        {
            return;
        }

        GameObject spearInstance = _resourceService.Instantiate(_spear);
        if (spearInstance == null)
        {
            return;
        }

        Projectile projectile = spearInstance.GetComponent<Projectile>();
        if (projectile == null || _sensor.DetectionTarget == null)
        {
            return;
        }

        Vector2 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.InitProjectile(transform.position, direction, angle, SPEAR_SPEED, _attackDamage);
    }
}
