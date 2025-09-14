using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NewGame : UI_Base
{
    private ISaveService _saveService;
    private IUIService _uiService;

    private enum GameObjects
    {
        P1Slot,
        P2Slot,
        P3Slot,
    }
    
    private enum Images
    {
        P1,
        P2,
        P3
    }

    private enum Buttons
    {
        DetermineButton,
        BackButton
    }

    private (Image slotImage, Image characterImage, Animator characterAnimator)[] _slots = new (Image, Image, Animator)[3];
    private ISceneService _sceneService;
    private GameObject _p1Slot;
    private GameObject _p2Slot;
    private GameObject _p3Slot;
    [SerializeField] private Animator _p1Animator;
    [SerializeField] private Animator _p2Animator;
    [SerializeField] private Animator _p3Animator;

    private int _currentSelectedSlot = -1;
    private const string NOT_SELECT_SLOT_COLOR = "#ABABAB";
    
    public override void EnsureService()
    {
        _saveService = ServiceLocator.GetService<ISaveService>();
        _sceneService = ServiceLocator.GetService<ISceneService>();
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
        BindEvent();
        SetInitailStatus(); 
    }
    
    public override void Refresh()
    {
        SetInitailStatus();
        _currentSelectedSlot = -1;
    }

    private void OnSlotClicked(PointerEventData data)
    {
        int clicked = data.pointerClick.gameObject switch
        {
            var slot1 when slot1 == _p1Slot => 1,
            var slot2 when slot2 == _p2Slot => 2,
            var slot3 when slot3 == _p3Slot => 3,
            _ => -1
        };

        if (clicked == -1 || clicked == _currentSelectedSlot)
        {
            return;
        }

        Define.EffectSoundType appearSound = clicked switch
        {
            1 => Define.EffectSoundType.P1_Appear,
            2 => Define.EffectSoundType.P2_Appear,
            3 => Define.EffectSoundType.P3_Appear,
            _ => Define.EffectSoundType.UI_Click
        };

        UpdateSlotVisual(clicked);
        _currentSelectedSlot = clicked;
        _soundService.PlayEffect(appearSound);
    }

    private void UpdateSlotVisual(int select)
    {
        int check = select - 1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i == check)
            {
                _slots[i].slotImage.color = Color.white;
                _slots[i].characterImage.color = Color.white;
                _slots[i].characterAnimator.Play($"P{i + 1}Appear");
            }
            else
            {
                _slots[i].slotImage.color = Util.GetColorFromHex(NOT_SELECT_SLOT_COLOR);
                _slots[i].characterImage.color = new Color(1, 1, 1, 0.0f);
            }
        }
    }

    private void OnDetermineButtonClicked(PointerEventData data)
    {
        if (_currentSelectedSlot == -1)
        {
            return;
        }

        if (_saveService.CreateNewData(_currentSelectedSlot) != null)
        {
            _sceneService.LoadScene(Define.SceneType.Lobby);
            _uiService.CloseUI(this.gameObject);
        }
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        _uiService.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _p1Slot = GetObject((int)GameObjects.P1Slot);
        _p2Slot = GetObject((int)GameObjects.P2Slot);
        _p3Slot = GetObject((int)GameObjects.P3Slot);

        _slots[0] = (_p1Slot.GetOrAddComponent<Image>(), GetImage((int)Images.P1), _p1Animator);
        _slots[1] = (_p2Slot.GetOrAddComponent<Image>(), GetImage((int)Images.P2), _p2Animator);
        _slots[2] = (_p3Slot.GetOrAddComponent<Image>(), GetImage((int)Images.P3), _p3Animator);
    }

    private void BindEvent()
    {
        GetButton((int)Buttons.DetermineButton).gameObject.BindEvent(OnDetermineButtonClicked);
        GetButton((int)Buttons.BackButton).gameObject.BindEvent(OnExitButtonClicked);

        _p1Slot.BindEvent(OnSlotClicked);
        _p2Slot.BindEvent(OnSlotClicked);
        _p3Slot.BindEvent(OnSlotClicked);
    }

    private void SetInitailStatus()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].slotImage.color = Util.GetColorFromHex(NOT_SELECT_SLOT_COLOR);
            _slots[i].characterImage.color = new Color(1, 1, 1, 0.0f);
        }
    }
}
