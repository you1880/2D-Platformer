using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputService
{
    event Action<Define.KeyboardEvent> KeyAction;
    event Action<Define.MouseEvent> MouseAction;
    bool IsShiftPressed { get; }
    bool IsSpacePressed { get; }
    void InputAction();
    void AddOrChangeKeyBind(Define.KeyboardEvent keyboardEvent, KeyCode key, Define.KeyInputType keyInputType);
    void RemoveKeyBind(Define.KeyboardEvent keyboardEvent);
}

