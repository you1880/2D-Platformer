using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{   
    private IUserDataService _userDataService;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private LayerMask _hittingPoint;
    private const float KNOCKBACK_FORCE = 7.0f;

    private void Start()
    {
        _userDataService = ServiceLocator.GetService<IUserDataService>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _hittingPoint) == 0)
        {
            return;
        }

        if (transform.position.y < collision.transform.position.y)
        {
            return;
        }

        BaseEnemy enemy = collision.transform.parent.GetComponent<BaseEnemy>();
        if (enemy == null)
        {
            return;
        }

        if (enemy.TakeDamage(_userDataService.Attack))
        {
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            _rigidbody2D.AddForce(knockbackDir * KNOCKBACK_FORCE, ForceMode2D.Impulse);
        }
    }
}
