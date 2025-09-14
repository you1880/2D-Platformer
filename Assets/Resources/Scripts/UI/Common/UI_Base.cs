using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    [SerializeField] protected UI_Animation _uiAnimatior;
    [SerializeField] protected RectTransform _rectTransform;
    protected ISoundService _soundService;

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];

        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = Util.FindChild(gameObject, names[i]);
            }
            else
            {
                objects[i] = Util.FindChild<T>(gameObject, names[i]);
            }

            if (objects[i] == null)
            {
                Debug.Log($"Failed to Bind : {names[i]}");
            }
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if (!_objects.TryGetValue(typeof(T), out objects))
        {
            return null;
        }

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) => Get<GameObject>(idx);
    protected TextMeshProUGUI GetText(int idx) => Get<TextMeshProUGUI>(idx);
    protected Image GetImage(int idx) => Get<Image>(idx);
    protected Button GetButton(int idx, bool isButtonSound = true)
    {
        Button btn = Get<Button>(idx);

        if (isButtonSound && btn != null)
        {
            btn.gameObject.BindEvent(BindButtonSound);
        }

        return btn;
    }

    public static void BindEvent(GameObject obj, Action<PointerEventData> action, Define.UIEvent eventType = Define.UIEvent.Click)
    {
        UI_EventHandler eventHandler = obj.GetOrAddComponent<UI_EventHandler>();

        switch (eventType)
        {
            case Define.UIEvent.Click:
                eventHandler.OnClickHandler -= action;
                eventHandler.OnClickHandler += action;
                break;
            case Define.UIEvent.PointerEnter:
                eventHandler.OnPointerEnterHandler -= action;
                eventHandler.OnPointerEnterHandler += action;
                break;
            case Define.UIEvent.PointerExit:
                eventHandler.OnPointerExitHandler -= action;
                eventHandler.OnPointerExitHandler += action;
                break;
        }
    }

    public abstract void EnsureService();
    public abstract void Init();
    public virtual void Clear() { }
    public virtual void Refresh() { }
    public virtual void Show()
    { 
        if (_uiAnimatior != null && _rectTransform != null)
        {
            StartCoroutine(_uiAnimatior.ShowUIScaleIn(_rectTransform));
        }
    }

    private void EnsureSoundService()
    {
        _soundService = ServiceLocator.GetService<ISoundService>();
    }

    private void BindButtonSound(PointerEventData data)
    {
        _soundService.PlayEffect(Define.EffectSoundType.UI_Click);
    }   

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ServiceLocator.IsInitialized);

        EnsureSoundService();
        EnsureService();
        Init();
    }

    private void OnDestroy()
    {
        Clear();
    }
}
