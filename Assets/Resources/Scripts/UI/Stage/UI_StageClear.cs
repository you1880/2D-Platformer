using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StageClear : UI_Base
{   
    private IDataService _dataService;
    private ISceneService _sceneService;
    private IStageService _stageService;

    private enum Texts
    {
        StageClearText,
        TimeText,
        AddGold,
        ExpText
    }

    private enum Buttons
    {
        ExitButton
    }

    private TextMeshProUGUI _stageClearText;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _addGoldText;
    private TextMeshProUGUI _expText;

    public override void EnsureService()
    {
        _dataService = ServiceLocator.GetService<IDataService>();
        _sceneService = ServiceLocator.GetService<ISceneService>();
        _stageService = ServiceLocator.GetService<IStageService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        ShowTimerText();
        SetInfo();
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        _sceneService.LoadScene(Define.SceneType.Lobby);
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _stageClearText = GetText((int)Texts.StageClearText);
        _timeText = GetText((int)Texts.TimeText);
        _addGoldText = GetText((int)Texts.AddGold);
        _expText = GetText((int)Texts.ExpText);
    }

    private void BindButtonEvent()
    {
        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void ShowTimerText()
    {
        if(_sceneService.CurrentScene is StageScene stageScene)
        {
            float _timer = stageScene.GetTimer();
            int minutes = (int)(_timer / 60);
            int seconds = (int)(_timer % 60);

            _timeText.text = $"걸린 시간 : {minutes:D2}:{seconds:D2}";
        }
    }

    private void SetInfo()
    {
        Data.Game.StageData stageData = _dataService.GetStageData(_stageService.CurrentStage);
        if (stageData == null)
        {
            Debug.LogError($"No Stage Data for {_stageService.CurrentStage}");
            return;
        }

        int head = (int)_stageService.CurrentStage / 10;
        int tail = (int)_stageService.CurrentStage % 10;

        _stageClearText.text = $"Stage {head}-{tail} Clear!";
        _addGoldText.text = $"+ {stageData.clearGold}";
        _expText.text = $"Exp + {stageData.clearExp}";
    }
}
