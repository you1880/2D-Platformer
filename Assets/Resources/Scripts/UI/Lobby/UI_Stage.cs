using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSlot
{
    private const string BUTTON_FRAME = "Frame";
    public Image FrameImage;
    public Button StageButton;

    public StageSlot(Image frameImage, Button button)
    {
        FrameImage = frameImage;
        StageButton = button;
    }

    public static StageSlot CreateStageSlot(GameObject slot)
    {
        Image frameImage = Util.FindChild<Image>(slot, BUTTON_FRAME, true);
        Button button = slot.GetComponent<Button>();

        return new StageSlot(frameImage, button);
    }

    public void SetSlotData(bool isClear, Sprite clearSprite)
    {
        if (FrameImage == null)
        {
            return;
        }

        if (isClear)
        {
            FrameImage.sprite = clearSprite;
        }
    }
}

public class UI_Stage : UI_Base
{
    private IStageService _stageService;
    private IUIService _uiService;

    private enum GameObjects
    {
        Stage1,
        Stage2,
        Stage1Contents,
        Stage2Contents,
        StageEnterUI
    }

    private enum Texts
    {
        EnterStageText,
        StageClearLogText
    }

    private enum Images
    {
        EnterMarkerImage,
        ClearMarkerImage
    }

    private enum Buttons
    {
        Stage1Button,
        Stage2Button,
        ExitButton,
        SkipButton,
        EnterButton,
        EnterUIExitButton
    }

    [SerializeField] private Sprite _clearStageSprite;
    [SerializeField] private Sprite _okMarkerSprite;
    [SerializeField] private Sprite _noMarkerSprite;
    private Dictionary<Define.Stage, StageSlot> _stageSlotDict = new Dictionary<Define.Stage, StageSlot>();
    private GameObject _stage1Panel;
    private GameObject _stage2Panel;
    private GameObject _stage1Contents;
    private GameObject _stage2Contents;
    private GameObject _stageEnterUI;
    private TextMeshProUGUI _enterStageText;
    private TextMeshProUGUI _stageClearLogText;
    private Image _enterMakerImage;
    private Image _clearMarkerImage;
    private Button _skipButton;
    private Button _enterButton;
    private Button _enterUIExitButton;
    private int _currentStageTab;
    private Define.Stage _clickedStage = Define.Stage.None;
    private const string STAGE_SLOT_PREFIX = "Stage";

    public override void EnsureService()
    {
        _stageService = ServiceLocator.GetService<IStageService>();
        _uiService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        CreateStageSlots(_stage1Contents);
        CreateStageSlots(_stage2Contents);
        BindButtonEvent();
        OnStageButtonClicked(null, 1);
        _stageEnterUI.SetActive(false);
    }

    private Define.Stage GetStageType(string buttonName)
    {
        if (buttonName.StartsWith(STAGE_SLOT_PREFIX))
        {
            string stageNoStr = buttonName.Substring(STAGE_SLOT_PREFIX.Length);
            if (int.TryParse(stageNoStr, out int stageNo))
            {
                return (Define.Stage)stageNo;
            }
        }

        return Define.Stage.None;
    }

    private void OnStageButtonClicked(PointerEventData data)
    {
        Define.Stage stageType = GetStageType(data.pointerPress.name);

        if (stageType == Define.Stage.None)
            return;

        if (_stageEnterUI != null)
        {
            _stageEnterUI.SetActive(true);
            SetEnterUI(stageType);
        }
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        _uiService.CloseUI(this.gameObject);
    }

    private void OnStageButtonClicked(PointerEventData data, int stage)
    {
        if (_currentStageTab == stage)
            return;

        _currentStageTab = stage;

        _stage1Panel.SetActive(stage == 1);
        _stage2Panel.SetActive(stage == 2);
    }

    private void SetEnterUI(Define.Stage stage)
    {
        _stageSlotDict.TryGetValue(stage, out StageSlot slot);
        if (slot == null)
            return;

        int head = (int)stage / 10; int tail = (int)stage % 10;

        _enterStageText.text = $"Enter Stage {head}-{tail}";
        _enterMakerImage.sprite = _stageService.CanEnterStage(stage) ? _okMarkerSprite : _noMarkerSprite;
        _clearMarkerImage.sprite = _stageService.IsStageClear(stage) ? _okMarkerSprite : _noMarkerSprite;
        _stageClearLogText.text = $"최단 기록 : {_stageService.GetStageClearTime(stage)}";
        _clickedStage = stage;
    }

    private void OnSkipButtonClicked(PointerEventData data)
    {
        if (_stageService.SkipStage(_clickedStage))
        {
            
        }
        _stageEnterUI.SetActive(false);
    }

    private void OnEnterButtonClicked(PointerEventData data)
    {
        if (_stageService.CanEnterStage(_clickedStage))
        {
            _stageService.EnterStage(_clickedStage);
        }
        
        DisableEnterUI();
    }

    private void OnEnterUIExitButtonClicked(PointerEventData data)
    {
        DisableEnterUI();
    }

    private void DisableEnterUI()
    {
        _stageEnterUI.SetActive(false);
        _clickedStage = Define.Stage.None;
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _stage1Panel = GetObject((int)GameObjects.Stage1);
        _stage2Panel = GetObject((int)GameObjects.Stage2);
        _stage1Contents = GetObject((int)GameObjects.Stage1Contents);
        _stage2Contents = GetObject((int)GameObjects.Stage2Contents);
        _stageEnterUI = GetObject((int)GameObjects.StageEnterUI);

        _enterStageText = GetText((int)Texts.EnterStageText);
        _stageClearLogText = GetText((int)Texts.StageClearLogText);

        _enterMakerImage = GetImage((int)Images.EnterMarkerImage);
        _clearMarkerImage = GetImage((int)Images.ClearMarkerImage);

        _skipButton = GetButton((int)Buttons.SkipButton);
        _enterButton = GetButton((int)Buttons.EnterButton);
        _enterUIExitButton = GetButton((int)Buttons.EnterUIExitButton);
    }

    private void CreateStageSlots(GameObject parent)
    {
        foreach (Transform transform in parent.transform)
        {
            Define.Stage stage = GetStageType(transform.name);
            if (stage == Define.Stage.None)
                continue;

            GameObject slotObject = transform.gameObject;
            StageSlot slot = StageSlot.CreateStageSlot(slotObject);
            if (slot != null)
            {
                bool isClear = _stageService.IsStageClear(stage);

                slot.SetSlotData(isClear, _clearStageSprite);
                slotObject.BindEvent(OnStageButtonClicked);
                _stageSlotDict.Add(stage, slot);
            }
        }
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.Stage1Button).gameObject.BindEvent((data) => OnStageButtonClicked(data, 1));
        GetButton((int)Buttons.Stage2Button).gameObject.BindEvent((data) => OnStageButtonClicked(data, 2));
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);

        _skipButton.gameObject.BindEvent(OnSkipButtonClicked);
        _enterButton.gameObject.BindEvent(OnEnterButtonClicked);
        _enterUIExitButton.gameObject.BindEvent(OnEnterUIExitButtonClicked);
    }
}
