using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _sensorCollider2D;
    private bool _sensorOn = false;
    private GameObject _detectionTarget;

    public GameObject DetectionTarget => _detectionTarget;
    public bool IsSensorOn => _sensorOn;

    private void Start()
    {
        if (_sensorCollider2D == null)
            _sensorCollider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _detectionTarget = collision.gameObject;
            _sensorOn = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _detectionTarget = null;    
            _sensorOn = false;
        }
    }
}
