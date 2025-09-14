using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement
{
    private readonly BaseEnemy _self;
    private readonly Rigidbody2D _rigidbody;
    private bool _isFacingRight = false;
    private LayerMask _wallLayer = 1 << LayerMask.GetMask("Wall");
    private float _moveSpeed = 1.0f;
    private float _groundCheckDistance = 1.0f;
    private LayerMask _groundLayer;

    public EnemyMovement(BaseEnemy self, Rigidbody2D rigidbody)
    {
        _self = self;
        _rigidbody = rigidbody;
        _groundLayer = LayerMask.GetMask("Ground");
    }

    public void Move()
    {
        float direction = _isFacingRight ? 1.0f : -1.0f;

        Vector2 frontGroundCheck = _rigidbody.position + new Vector2(direction * 0.5f, 0f);
        bool isGroundAhead = Physics2D.Raycast(frontGroundCheck, Vector2.down, _groundCheckDistance, _groundLayer);

        if (IsWallAhead() || !isGroundAhead)
        {
            ChangeDirection();
            direction *= -1.0f;
        }

        _rigidbody.velocity = new Vector2(direction * _moveSpeed, _rigidbody.velocity.y);
    }

    public void ChangeAttackDirection(Transform target, Transform attacker)
    {
        bool targetOnRight = target.transform.position.x > attacker.transform.position.x;
        if (targetOnRight != _isFacingRight)
        {
            ChangeDirection();
        }
    }

    private bool IsWallAhead()
    {
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(_rigidbody.position, direction, 1.0f, _wallLayer).collider != null;
    }

    private void ChangeDirection()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = _rigidbody.transform.localScale;
        scale.x *= -1;
        _rigidbody.transform.localScale = scale;
    }
}
