using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private IResourceService _resourceService;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _groundLayer;
    private Vector2 _direction;
    private float _angle;
    private float _projectileSpeed = 5.0f;
    private float _lifetime = 0.0f;
    private int _damage;
    private const float MAX_LIFETIME = 10.0f;

    public void InitProjectile(Vector3 position, Vector2 direction, float angle, float speed, int damage)
    {
        transform.position = position;
        _direction = direction.normalized;
        _angle = angle;
        _projectileSpeed = speed;
        _damage = damage;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, _angle);
    }

    private void Start()
    {
        _resourceService = ServiceLocator.GetService<IResourceService>();
    }

    private void Update()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime >= MAX_LIFETIME)
        {
            _resourceService.Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = _direction * _projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            _resourceService.Destroy(gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            PlayerController controller = collision.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.Hit(_damage);
                _resourceService.Destroy(gameObject);
            }
        }
    }
}
