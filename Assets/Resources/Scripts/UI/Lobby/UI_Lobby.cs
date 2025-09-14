using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Lobby : UI_Base
{
    private ISaveService _saveService;
    private ISceneService _sceneService;
    private IUIService _uiService;

    private enum Buttons
    {
        Stage,
        Stat,
        Shop,
        Save,
        Setting,
        Title
    }

    private enum PanelType { Stage, Stat, Shop, Save, Setting, Title };
    private Button[] _panels = new Button[6];
    private RectTransform[] _panelRects = new RectTransform[6];
    private Coroutine[] _panelCoroutines = new Coroutine[6];
    private const float ORIGINAL_Y_POS = 50.0f;
    private const float MAX_Y_POS = 100.0f;

    public override void EnsureService()
    {
        _saveService = ServiceLocator.GetService<ISaveService>();
        _sceneService = ServiceLocator.GetService<ISceneService>();
        _uiService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
    }

    public override void Show() {}

    private void OnStageButtonClicked(PointerEventData data)
    {
        _uiService.ShowUI<UI_Stage>();
    }

    private void OnStatButtonClicked(PointerEventData data)
    {
        _uiService.ShowUI<UI_Stat>();
    }

    private void OnShopButtonClicked(PointerEventData data)
    {
        _uiService.ShowUI<UI_Shop>();
    }

    // private void OnInventoryClicked(PointerEventData data)
    // {
    //     //_uiService.ShowUI<UI_Inventory>();
    // }

    private void OnSaveButtonClicked(PointerEventData data)
    {
        _uiService.ShowMessageBox(MessageID.SaveData, SaveButtonAction);
    }

    private void SaveButtonAction()
    {
        if (_saveService.Save())
        {
            _uiService.ShowMessageBox(MessageID.SaveDataSuccess);
        }
        else
        {
            _uiService.ShowMessageBox(MessageID.SaveDataError);
        }
    }

    private void OnSettingButtonClicked(PointerEventData data)
    {
        _uiService.ShowUI<UI_Setting>();
    }

    private void OnTitleButtonClicked(PointerEventData data)
    {
        _uiService.ShowMessageBox(MessageID.GoTitle, TitleButtonAction);
    }

    private void TitleButtonAction()
    {
        _sceneService.LoadScene(Define.SceneType.Title);
    }

    private void OnPanelEntered(PointerEventData data)
    {
        int panel = Enum.TryParse(data.pointerEnter.name, out PanelType panelType) ? (int)panelType : -1;
        if (panel < 0 || panel >= _panels.Length)
            return;

        if (_panelCoroutines[panel] != null)
            StopCoroutine(_panelCoroutines[panel]);
        _panelCoroutines[panel] = StartCoroutine(_uiAnimatior.MoveUp(_panelRects[panel], MAX_Y_POS, 0.5f));
    }

    private void OnPanelExited(PointerEventData data)
    {
        int panel = Enum.TryParse(data.pointerEnter.name, out PanelType panelType) ? (int)panelType : -1;
        if (panel < 0 || panel >= _panels.Length)
            return;

        if (_panelCoroutines[panel] != null)
            StopCoroutine(_panelCoroutines[panel]);
        _panelCoroutines[panel] = StartCoroutine(_uiAnimatior.MoveUp(_panelRects[panel], ORIGINAL_Y_POS, 0.5f));
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        foreach (Buttons type in Enum.GetValues(typeof(Buttons)))
        {
            _panels[(int)type] = GetButton((int)type);
            if (_panels[(int)type] != null)
            {
                _panelRects[(int)type] = _panels[(int)type].GetComponent<RectTransform>();
            }
        }
    }

    private void BindButtonEvent()
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            PanelType panelType = (PanelType)i;
            Action<PointerEventData> onClickAction = panelType switch
            {
                PanelType.Stage => OnStageButtonClicked,
                PanelType.Stat => OnStatButtonClicked,
                PanelType.Shop => OnShopButtonClicked,
                //PanelType.Inventory => OnInventoryClicked,
                PanelType.Save => OnSaveButtonClicked,
                PanelType.Setting => OnSettingButtonClicked,
                PanelType.Title => OnTitleButtonClicked,
                _ => null
            };


            if (onClickAction != null)
            {
                _panels[i].gameObject.BindEvent(onClickAction);
            }
            _panels[i].gameObject.BindEvent(OnPanelEntered, Define.UIEvent.PointerEnter);
            _panels[i].gameObject.BindEvent(OnPanelExited, Define.UIEvent.PointerExit);
        }
    }
}
