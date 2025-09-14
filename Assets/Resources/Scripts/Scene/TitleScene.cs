using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    [SerializeField] private GameObject _backgrounds;
    private Vector3 _startPos;
    private float _elapsedTime;
    private const float BACKGROUND_MOVE_SPEED = 0.01f;
    private const float MOVE_RANGE = 2.0f;

    public override void Clear() { }

    public override void Init()
    {
        SceneType = Define.SceneType.Title;
        _soundService.PlayBGM(SceneType);

        if (_backgrounds != null)
        {
            _startPos = _backgrounds.transform.position;
        }

        InstantiateUI();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        float offset = Mathf.Sin(_elapsedTime * BACKGROUND_MOVE_SPEED) * MOVE_RANGE;
        _backgrounds.transform.position = new Vector3(_startPos.x + offset, _startPos.y, _startPos.z);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            InstantiateUI();
        }
    }

    private void InstantiateUI()
    {
        UI_Base ui = _uiService.ShowUI<UI_Title>();
    }
}
