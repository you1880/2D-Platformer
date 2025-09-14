using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hp : UI_Base
{
    private IUserDataService _userDataService;

    private enum GameObjects
    {
        HpSlider
    }

    private enum Images
    {
        PlayerImage
    }

    private enum Texts
    {
        HpValue
    }

    [SerializeField] private Sprite[] _playerSprites;
    private Slider _hpSlider;
    private Image _playerImage;
    private TextMeshProUGUI _hpValue;

    public override void EnsureService()
    {
        _userDataService = ServiceLocator.GetService<IUserDataService>();
    }

    public override void Init()
    {
        BindUIElments();
        GetUIElements();
        SetPlayerImage();
        SetHpSlider(_userDataService.HP);
        BindEvent();
    }

    private void BindUIElments()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void GetUIElements()
    {
        GameObject slider = GetObject((int)GameObjects.HpSlider);
        if (slider != null)
            _hpSlider = slider.GetComponent<Slider>();

        _playerImage = GetImage((int)Images.PlayerImage);
        _hpValue = GetText((int)Texts.HpValue);
    }

    private void SetPlayerImage()
    {
        int playerType = _userDataService.PlayerType - 1;

        if (playerType >= 0 && playerType < _playerSprites.Length)
            _playerImage.sprite = _playerSprites[playerType];
    }

    private void SetHpSlider(int currentHp)
    {
        int maxHp = _userDataService.MaxHP;

        if (_hpSlider != null)
        {
            _hpSlider.maxValue = maxHp;
            _hpSlider.value = currentHp;
        }

        if (_hpValue != null)
            _hpValue.text = $"{currentHp} / {maxHp}";
    }

    private void BindEvent()
    {
        _userDataService.OnHpChanged += SetHpSlider;
    }

    private void UnbindEvent()
    {
        _userDataService.OnHpChanged -= SetHpSlider;
    }

    private void OnDisable()
    {
        UnbindEvent();
    }
}
