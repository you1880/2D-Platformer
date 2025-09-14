using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MessageBox : UI_Base
{
    private IUIService _uiService;

    private enum Texts
    {
        Message
    }

    private enum Buttons
    {
        ConfirmButton,
        CancelButton
    }

    private TextMeshProUGUI _messageText;
    private Action _callback;
    private string _text;
    private bool _isConfirmMode;

    public override void EnsureService()
    {
        _uiService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        BindUIElements();
        BindButtonEvent();
        GetUIElements();
    }

    public override void Clear()
    {
        _callback = null;
        _text = string.Empty;
        _isConfirmMode = false;
    }

    public void SetMessageBox(string text, Action onCompleted = null, bool isConfirmMode = false)
    {
        _text = text;
        _callback = onCompleted;
        _isConfirmMode = isConfirmMode;

        if (_messageText != null)
        {
            _messageText.text = _text;
        }
    }

    private void OnConfirmButtonClicked(PointerEventData data)
    {
        _callback?.Invoke();
        _uiService.CloseMessageBox(this.gameObject);
    }

    private void OnCancelButtonClicked(PointerEventData data)
    {
        _uiService.CloseMessageBox(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnConfirmButtonClicked);
        Button cancelButton = GetButton((int)Buttons.CancelButton);

        if (!_isConfirmMode)
        {
            cancelButton.gameObject.BindEvent(OnCancelButtonClicked);
        }
        else
        {
            cancelButton.gameObject.SetActive(false);
        }
    }

    private void GetUIElements()
    {
        _messageText = GetText((int)Texts.Message);
        _messageText.text = _text;
    }

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    private void OnDisable()
    {
        Clear();
    }
}
