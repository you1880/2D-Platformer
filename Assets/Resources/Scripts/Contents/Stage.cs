using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private GameObject _characterSpawnPostion;
    [SerializeField] private BoxCollider2D _areaBound;
    private const float CORRECTION_Y = 0.15f;

    public BoxCollider2D AreaBound => _areaBound;
    
    public Vector3 CharacterSpawnPosition
    {
        get
        {
            if (_characterSpawnPostion == null)
            {
                return Vector3.zero;
            }

            return new Vector3(_characterSpawnPostion.transform.position.x,
                _characterSpawnPostion.transform.position.y + CORRECTION_Y, _characterSpawnPostion.transform.position.z);
        }
    }
}
