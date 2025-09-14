using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();

        if (component == null)
        {
            component = obj.AddComponent<T>();
        }

        return component;
    }

    public static void BindEvent(this GameObject obj, Action<PointerEventData> action, Define.UIEvent eventType = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(obj, action, eventType);
    }
}
