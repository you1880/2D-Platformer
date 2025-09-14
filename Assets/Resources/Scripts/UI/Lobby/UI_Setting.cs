using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    private IUIService _uIService;

    private enum GameObjects
    {
        GraphicSetting,
        SoundSetting,
        WindowedToggle,
        ResolutionDropdown,
        BgmSlider,
        EffectSlider
    }

    private enum Texts
    {
        BgmValue,
        EffectValue
    }

    private enum Buttons
    {
        GraphicButton,
        SoundButton,
        ExitButton
    }

    private enum SettingSlot { Graphic, Sound };
    private List<Resolution> _resolutions;
    private GameObject _graphicSetting;
    private GameObject _soundSetting;
    private Toggle _windowedToggle;
    private TMP_Dropdown _resolutionDropdown;
    private Slider _bgmSlider;
    private Slider _effectSlider;
    private TextMeshProUGUI _bgmValue;
    private TextMeshProUGUI _effectValue;
    private SettingSlot _currentSlot = SettingSlot.Graphic;

    public override void EnsureService()
    {
        _uIService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InitToggle();
        InitResolutionDropdown();
        InitSoundSlider();
        ShowCurrentSetting();
    }

    private void OnSettingButtonClicked(PointerEventData data, SettingSlot slot)
    {
        if (_currentSlot == slot)
            return;

        _currentSlot = slot;
        ShowCurrentSetting();
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        _uIService.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _graphicSetting = GetObject((int)GameObjects.GraphicSetting);
        _soundSetting = GetObject((int)GameObjects.SoundSetting);
        _windowedToggle = GetObject((int)GameObjects.WindowedToggle)?.GetComponent<Toggle>();
        _resolutionDropdown = GetObject((int)GameObjects.ResolutionDropdown)?.GetComponent<TMP_Dropdown>();
        _bgmSlider = GetObject((int)GameObjects.BgmSlider)?.GetComponent<Slider>();
        _effectSlider = GetObject((int)GameObjects.EffectSlider)?.GetComponent<Slider>();
        _bgmValue = GetText((int)Texts.BgmValue);
        _effectValue = GetText((int)Texts.EffectValue);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.GraphicButton).gameObject.BindEvent((data) => OnSettingButtonClicked(data, SettingSlot.Graphic));
        GetButton((int)Buttons.SoundButton).gameObject.BindEvent((data) => OnSettingButtonClicked(data, SettingSlot.Sound));
    }

    private void OnToggleChanged(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isFullScreen;
    }

    private void InitToggle()
    {
        _windowedToggle.isOn = Screen.fullScreen;
        _windowedToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDropdownChanged(int index)
    {
        if (index < 0 || index >= _resolutions.Count)
            return;

        Resolution res = _resolutions[index];
        bool isFullScreen = Screen.fullScreen;

        Screen.SetResolution(res.width, res.height, isFullScreen);
    }

    private void InitResolutionDropdown()
    {
        _resolutions = new List<Resolution>(Screen.resolutions);
        _resolutionDropdown.ClearOptions();

        int currentIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < _resolutions.Count; i++)
        {
            Resolution res = _resolutions[i];
            string option = $"{res.width} x {res.height} @ {res.refreshRate}Hz";

            options.Add(option);

            if (res.width == Screen.width &&
                res.height == Screen.height
                && res.refreshRate == Screen.currentResolution.refreshRate)
            {
                currentIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentIndex;
        _resolutionDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnBgmSliderChanged(float value)
    {
        _soundService.SetBgmVolume(value);
        _bgmValue.text = $"{value}";
    }

    private void OnEffectSliderChanged(float value)
    {
        _soundService.SetEffectVolume(value);
        _effectValue.text = $"{value}";
    }

    private void InitSoundSlider()
    {
        _bgmSlider.wholeNumbers = _effectSlider.wholeNumbers = true;
        _bgmSlider.minValue = _effectSlider.minValue = 0;
        _bgmSlider.maxValue = _effectSlider.maxValue = 100;

        _bgmSlider.value = _soundService.BgmVolume * 100.0f;
        _effectSlider.value = _soundService.EffectVolume * 100.0f;

        _bgmValue.text = $"{_bgmSlider.value}";
        _effectValue.text = $"{_effectSlider.value}";

        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        _effectSlider.onValueChanged.AddListener(OnEffectSliderChanged);
    }

    private void ShowCurrentSetting()
    {
        _graphicSetting.SetActive(_currentSlot == SettingSlot.Graphic);
        _soundSetting.SetActive(_currentSlot == SettingSlot.Sound);
    }
}






