using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    private Bounds _bounds;
    private float _halfHeight;
    private float _halfWidth;

    public void InitailizeCamera(GameObject player, BoxCollider2D collider2D)
    {
        _player = player;
        _bounds = collider2D.bounds;
    }

    private void Start()
    {
        _halfHeight = Camera.main.orthographicSize;
        _halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
    }

    void Update()
    {
        if (_player == null)
            return;

        Vector3 cameraPosition = new Vector3(_player.transform.position.x,
            _player.transform.position.y, -10.0f);

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, _bounds.min.x + _halfWidth, _bounds.max.x - _halfWidth);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, _bounds.min.y + _halfHeight, _bounds.max.y - _halfHeight);

        transform.position = cameraPosition;
    }
}
