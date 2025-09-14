using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Title : UI_Base
{
    private IUIService _uiService;

    private enum Texts
    {
        NewGameButtonText,
        LoadButtonText,
        QuitButtonText
    }

    private enum Buttons
    {
        NewGameButton,
        LoadButton,
        QuitButton
    }
    private TextMeshProUGUI _newGameButtonText;
    private TextMeshProUGUI _loadButtonText;
    private TextMeshProUGUI _quitButtonText;
    private Button _newGameButton;
    private Button _loadButton;
    private Button _quitButton;
    private Coroutine _textBlinkCoroutine = null;
    private TextMeshProUGUI _currentBlinkingText = null;

    public override void EnsureService()
    {
        _uiService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        if (_uiAnimatior == null)
        {
            _uiAnimatior = gameObject.GetOrAddComponent<UI_Animation>();
        }

        BindUIElements();
        GetUIElements();
        BindButtonEvent();
    }

    public override void Show()
    {
        if (_uiAnimatior != null && _rectTransform != null)
        {
            StartCoroutine(_uiAnimatior.ShowUIScaleIn(_rectTransform));
        }
    }

    private void OnButtonEntered(PointerEventData data)
    {
        if (data.pointerEnter.name == Enum.GetName(typeof(Texts), (int)Texts.NewGameButtonText))
        {
            _currentBlinkingText = _newGameButtonText;
        }
        else if (data.pointerEnter.name == Enum.GetName(typeof(Texts), (int)Texts.LoadButtonText))
        {
            _currentBlinkingText = _loadButtonText;
        }
        else if (data.pointerEnter.name == Enum.GetName(typeof(Texts), (int)Texts.QuitButtonText))
        {
            _currentBlinkingText = _quitButtonText;
        }

        if (_currentBlinkingText != null)
        {
            if (_textBlinkCoroutine != null)
            {
                _uiAnimatior.StopBlinkingText(_currentBlinkingText);
                StopCoroutine(_textBlinkCoroutine);
                _textBlinkCoroutine = null;
            }
            _textBlinkCoroutine = StartCoroutine(_uiAnimatior.BlinkText(_currentBlinkingText));
        }
    }

    private void OnButtonExited(PointerEventData data)
    {
        if (_textBlinkCoroutine != null)
        {
            _uiAnimatior.StopBlinkingText(_currentBlinkingText);
            StopCoroutine(_textBlinkCoroutine);
            _textBlinkCoroutine = null;
            _currentBlinkingText = null;
        }
    }

    private void OnNewGameButtonClicked(PointerEventData data)
    {
        _uiService.ShowUI<UI_NewGame>();
    }

    private void OnLoadButtonClicked(PointerEventData data)
    {
        _uiService.ShowUI<UI_Load>();
    }

    private void OnQuitButtonClicked(PointerEventData data)
    {
        Application.Quit();
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _newGameButtonText = GetText((int)Texts.NewGameButtonText);
        _loadButtonText = GetText((int)Texts.LoadButtonText);
        _quitButtonText = GetText((int)Texts.QuitButtonText);

        _newGameButton = GetButton((int)Buttons.NewGameButton);
        _loadButton = GetButton((int)Buttons.LoadButton);
        _quitButton = GetButton((int)Buttons.QuitButton);
    }

    private void BindButtonEvent()
    {
        _newGameButton.gameObject.BindEvent(OnNewGameButtonClicked);
        _newGameButton.gameObject.BindEvent(OnButtonEntered, Define.UIEvent.PointerEnter);
        _newGameButton.gameObject.BindEvent(OnButtonExited, Define.UIEvent.PointerExit);
        _loadButton.gameObject.BindEvent(OnLoadButtonClicked);
        _loadButton.gameObject.BindEvent(OnButtonEntered, Define.UIEvent.PointerEnter);
        _loadButton.gameObject.BindEvent(OnButtonExited, Define.UIEvent.PointerExit);
        _quitButton.gameObject.BindEvent(OnQuitButtonClicked);
        _quitButton.gameObject.BindEvent(OnButtonEntered, Define.UIEvent.PointerEnter);
        _quitButton.gameObject.BindEvent(OnButtonExited, Define.UIEvent.PointerExit);
    }
}
