using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyBind
{
    public KeyCode Key;
    public Define.KeyInputType InputType;

    public KeyBind(KeyCode key, Define.KeyInputType keyInputType)
    {
        Key = key;
        InputType = keyInputType;
    }
}

[AutoRegister(typeof(IInputService), priority: 10)]
public class InputService : IInputService
{
    private Dictionary<Define.KeyboardEvent, KeyBind> _keyMap = new Dictionary<Define.KeyboardEvent, KeyBind>
    {
        {Define.KeyboardEvent.Left, new KeyBind(KeyCode.A, Define.KeyInputType.Hold)},
        {Define.KeyboardEvent.Right, new KeyBind(KeyCode.D, Define.KeyInputType.Hold)},
    };

    private readonly HashSet<Define.KeyboardEvent> _movementKeys = new HashSet<Define.KeyboardEvent>
    {
        Define.KeyboardEvent.Left,
        Define.KeyboardEvent.Right,
    };

    private float _pressedTime = 0.0f;
    private const float CLICK_DELAY = 0.5f;
    public bool IsShiftPressed => Input.GetKey(KeyCode.LeftShift);
    public bool IsSpacePressed => Input.GetKeyDown(KeyCode.Space);
    public event Action<Define.KeyboardEvent> KeyAction = null;
    public event Action<Define.MouseEvent> MouseAction = null;

    public void InputAction()
    {
        KeyInputAction();
        MouseInputAction();
    }

    public void AddOrChangeKeyBind(Define.KeyboardEvent keyboardEvent, KeyCode key, Define.KeyInputType keyInputType = Define.KeyInputType.Down)
    {
        if (_keyMap.ContainsKey(keyboardEvent))
        {
            _keyMap[keyboardEvent] = new KeyBind(key, keyInputType);
        }
        else
        {
            _keyMap.Add(keyboardEvent, new KeyBind(key, keyInputType));
        }
    }

    public void RemoveKeyBind(Define.KeyboardEvent keyboardEvent)
    {
        _keyMap.Remove(keyboardEvent);
    }

    private bool IsMovementKeyPressed()
    {
        foreach (Define.KeyboardEvent moveKey in _movementKeys)
        {
            if (_keyMap.TryGetValue(moveKey, out KeyBind keyBind))
            {
                if (Input.GetKey(keyBind.Key))
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void KeyInputAction()
    {
        if (KeyAction == null)
        {
            return;
        }

        if (!IsMovementKeyPressed())
        {
            KeyAction?.Invoke(Define.KeyboardEvent.None);
            return;
        }

        foreach (var keyMap in _keyMap)
        {
            bool pressed = false;
            KeyBind keyBind = keyMap.Value;

            switch (keyBind.InputType)
            {
                case Define.KeyInputType.Down:
                    pressed = Input.GetKeyDown(keyBind.Key);
                    break;
                case Define.KeyInputType.Hold:
                    pressed = Input.GetKey(keyBind.Key);
                    break;
            }

            if (pressed)
            {
                KeyAction?.Invoke(keyMap.Key);
            }
        }
    }

    private void MouseInputAction()
    {
        if (MouseAction != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - _pressedTime >= CLICK_DELAY)
                {
                    MouseAction.Invoke(Define.MouseEvent.LClick);
                    _pressedTime = Time.time;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                MouseAction.Invoke(Define.MouseEvent.RClick);
            }
        }
    }
}
