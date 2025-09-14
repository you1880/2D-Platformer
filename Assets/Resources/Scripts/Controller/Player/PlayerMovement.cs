using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement
{
    private float _playerWalkSpeed = 2.0f;
    private float _playerRunSpeed = 3.5f;
    private int _playerDoubleJumpCount = 0;
    private Collider2D _playerCollider2D;
    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private LayerMask _groundLayer;
    private const float GROUND_ACCELATION = 60.0f;
    private const float GROUND_DECELATION = 70.0f;
    private const float AIR_ACCELATION = 30.0f;
    private const float AIR_DECELATION = 25.0f;
    private const float JUMP_FORCE = 4.0f;
    private const float COYOTE_TIME = 0.15f;
    private const float JUMP_BUFFER_TIME = 0.10f;
    private const float FALL_GRAVITY_MULTIPLIER = 1.8f;
    private const float GROUND_CHECK_DISTANCE = 0.1f;
    private const float CEILING_CHECK_DISTANCE = 0.05f;

    private float _coyoteTimer = 0.0f;
    private float _bufferTimer = 0.0f;
    private float _moveX;
    private int _jumpCount = 0;
    private bool _isRun;
    private bool _isJump;

    public PlayerMovement(Collider2D playerCollider2D, Rigidbody2D rigidbody, Transform transform, LayerMask groundLayer)
    {
        _playerCollider2D = playerCollider2D;
        _rigidbody = rigidbody;
        _transform = transform;
        _groundLayer = groundLayer;
    }

    public void SetMoveInfo(float moveX, bool isRun, bool isJumpPressed)
    {
        _moveX = moveX;
        _isRun = isRun;
        _isJump = isJumpPressed;
        if (isJumpPressed) _bufferTimer = JUMP_BUFFER_TIME;
    }

    public void Move()
    {
        bool isGround = IsGround();
        PlayerMove(isGround);
        Rotation(_moveX);
    }

    public void DecreaseBufferTimer()
    {
        if (_bufferTimer > 0.0f)
            _bufferTimer -= Time.unscaledDeltaTime;
    }

    private void PlayerMove(bool isGround)
    {
        if (isGround)
        {
            _coyoteTimer = COYOTE_TIME;
            _jumpCount = 0;
        }
        else if (_coyoteTimer > 0.0f)
        {
            _coyoteTimer -= Time.fixedDeltaTime;
        }

        float speed = _isRun ? _playerRunSpeed : _playerWalkSpeed;
        float targetSpeed = (Mathf.Abs(_moveX) > 0.01f) ? Mathf.Sign(_moveX) * speed : 0.0f;
        float currentSpeed = _rigidbody.velocity.x;
        bool isBraking = Mathf.Approximately(targetSpeed, 0.0f) || Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed);
        float accelation = isGround ? (isBraking ? GROUND_DECELATION : GROUND_ACCELATION) : (isBraking ? AIR_DECELATION : AIR_ACCELATION);
        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelation * Time.fixedDeltaTime);

        bool canJump = isGround || _coyoteTimer > 0.0f;
        bool canDoubleJump = !isGround && _jumpCount < _playerDoubleJumpCount;
        bool jump = _bufferTimer > 0.0f && (canJump || canDoubleJump);

        Vector2 vel = _rigidbody.velocity;

        if (jump)
        {
            if (_rigidbody.velocity.y < 0.0f) vel.y = 0.0f;

            vel.y = JUMP_FORCE;
            _rigidbody.velocity = new Vector2(newSpeed, vel.y);

            _bufferTimer = 0.0f;
            _coyoteTimer = 0.0f;

            if (!isGround)
                _jumpCount++;
        }
        else
        {
            if (_rigidbody.velocity.y < 0.0f)
                _rigidbody.velocity += Vector2.up * (Physics2D.gravity.y * (FALL_GRAVITY_MULTIPLIER - 1.0f) * Time.fixedDeltaTime);

            if (_rigidbody.velocity.y > 0.0f && IsCeiling())
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0.0f);

            _rigidbody.velocity = new Vector2(newSpeed, _rigidbody.velocity.y);
        }
    }

    private void Rotation(float moveX)
    {
        if (Mathf.Abs(moveX) > 0.01f)
        {
            _transform.localScale = new Vector3(Mathf.Sign(moveX), 1f, 1f);
        }
    }
    private bool IsGround()
    {
        Bounds bounds = _playerCollider2D.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0.0f, Vector2.down, GROUND_CHECK_DISTANCE, _groundLayer).collider != null;
    }

    private bool IsCeiling()
    {
        Bounds bounds = _playerCollider2D.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0.0f, Vector2.up, CEILING_CHECK_DISTANCE, _groundLayer).collider != null;
    }
}
