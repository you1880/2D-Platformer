using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _backgroundDuplicate;
    [SerializeField] private GameObject _midground;
    [SerializeField] private GameObject _midgroundDuplicate;
    [SerializeField] private GameObject _foreground;
    [SerializeField] private GameObject _foregroundDuplicate;

    private const float BACKGROUND_SCROLLING_SPEED = 0.95f;
    private const float MIDGROUND_SCROLLING_SPEED = 0.90f;
    private const float FOREGROUND_SCROLLING_SPEED = 0.80f;
    private const float BACKGROUND_WIDTH = 9.6f;

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;
    private Camera _mainCamera;
    private Dictionary<GameObject, GameObject> _duplicateBackgrounds = new();

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _mainCamera = Camera.main;
        _lastCameraPosition = _cameraTransform.position;

        LocateDuplicate(_background, _backgroundDuplicate);
        LocateDuplicate(_midground, _midgroundDuplicate);  
        LocateDuplicate(_foreground, _foregroundDuplicate);
    }

    private void LocateDuplicate(GameObject original, GameObject duplicate)
    {
        duplicate.transform.position = original.transform.position + Vector3.right * BACKGROUND_WIDTH;
        _duplicateBackgrounds[original] = duplicate;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;

        MoveLayerWithDuplicate(_background, deltaMovement, BACKGROUND_SCROLLING_SPEED);
        MoveLayerWithDuplicate(_midground, deltaMovement, MIDGROUND_SCROLLING_SPEED);
        MoveLayerWithDuplicate(_foreground, deltaMovement, FOREGROUND_SCROLLING_SPEED);

        _lastCameraPosition = _cameraTransform.position;
    }

    private void MoveLayerWithDuplicate(GameObject layer, Vector3 deltaMovement, float parallaxEffect)
    {
        float parallaxX = deltaMovement.x * parallaxEffect;
        Vector3 originalPos = layer.transform.position;
        originalPos.x += parallaxX;
        layer.transform.position = originalPos;

        GameObject duplicate = _duplicateBackgrounds[layer];
        Vector3 duplicatePos = duplicate.transform.position;
        duplicatePos.x += parallaxX;
        duplicate.transform.position = duplicatePos;

        float camX = _cameraTransform.position.x;
        float originalDistanceFromCam = Mathf.Abs(camX - originalPos.x);
        float duplicateDistanceFromCam = Mathf.Abs(camX - duplicatePos.x);

        if (originalDistanceFromCam > BACKGROUND_WIDTH * 1.5f)
        {
            layer.transform.position = duplicatePos + 
                (originalPos.x < camX ? Vector3.right : Vector3.left) * BACKGROUND_WIDTH;
        }
        else if (duplicateDistanceFromCam > BACKGROUND_WIDTH * 1.5f)
        {
            duplicate.transform.position = originalPos + 
                (duplicatePos.x < camX ? Vector3.right : Vector3.left) * BACKGROUND_WIDTH;
        }
    }
}
