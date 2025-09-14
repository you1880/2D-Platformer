using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIService
{
    GameObject Root { get; }
    T ShowUI<T>(string name = null) where T : UI_Base;
    UI_MessageBox ShowMessageBox(MessageID messageID, System.Action onCompleted = null, bool isConfirmMode = false);
    void CloseUI(GameObject ui);
    void CloseMessageBox(GameObject ui);
    void ClearUIDictionary();
}

