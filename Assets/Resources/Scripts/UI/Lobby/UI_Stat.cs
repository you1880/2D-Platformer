using System.Collections;
using System.Collections.Generic;
using Data.User;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Stat : UI_Base
{
    private IDataService _dataService;
    private IUserDataService _userDataService;
    private IUIService _uiService;

    private enum GameObjects
    {
        HpSlider,
        ExpSlider
    }

    private enum Images
    {
        P1,
        P2,
        P3
    }

    private enum Texts
    {
        LevelText,
        HpValue,
        ExpValue,
        AttackText,
        GoldText
    }

    private enum Buttons
    {
        ExitButton
    }

    private Slider _hpSlider;
    private Slider _expSlider;
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _hpValue;
    private TextMeshProUGUI _expValue;
    private TextMeshProUGUI _attackText;
    private TextMeshProUGUI _goldText;
    private Image _p1Image;
    private Image _p2Image;
    private Image _p3Image;

    public override void EnsureService()
    {
        _dataService = ServiceLocator.GetService<IDataService>();
        _userDataService = ServiceLocator.GetService<IUserDataService>();
        _uiService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        SetStatUI();
    }

    public override void Refresh()
    {
        SetStatUI();
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        _uiService.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        GameObject hpSlider = GetObject((int)GameObjects.HpSlider);
        if (hpSlider != null)
            _hpSlider = hpSlider.GetComponent<Slider>();
        GameObject expSlider = GetObject((int)GameObjects.ExpSlider);
        if (expSlider != null)
            _expSlider = expSlider.GetComponent<Slider>();

        _levelText = GetText((int)Texts.LevelText);
        _hpValue = GetText((int)Texts.HpValue);
        _expValue = GetText((int)Texts.ExpValue);
        _attackText = GetText((int)Texts.AttackText);
        _goldText = GetText((int)Texts.GoldText);
        _p1Image = GetImage((int)Images.P1);
        _p2Image = GetImage((int)Images.P2);
        _p3Image = GetImage((int)Images.P3);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void SetStatUI()
    {
        int playerType = _userDataService.PlayerType;
        int level = _userDataService.Level;
        int exp = _userDataService.Exp;
        int maxHp = _userDataService.MaxHP;
        int hp = _userDataService.HP;
        int attack = _userDataService.Attack;
        int gold = _userDataService.Gold;

        SetPlayerSprite(playerType);
        SetHpSlider(hp, maxHp);
        SetExpSlider(level, exp);

        _attackText.text = $"Attack : {attack}";
        _goldText.text = $"Gold : {gold:N0}";
    }

    private void SetPlayerSprite(int playerType)
    {
        if (_p1Image == null || _p2Image == null || _p3Image == null)
            return;

        _p1Image.color = new Color(1, 1, 1, 0.0f);
        _p2Image.color = new Color(1, 1, 1, 0.0f);
        _p3Image.color = new Color(1, 1, 1, 0.0f);

        switch (playerType)
        {
            case 1:
                _p1Image.color = Color.white;
                break;
            case 2:
                _p2Image.color = Color.white;
                break;
            case 3:
                _p3Image.color = Color.white;
                break;
        }
    }

    private void SetExpSlider(int level, int exp)
    {
        Data.Game.RequireExp req = _dataService.GetRequireExp(level);
        if (req == null || _expSlider == null || _expValue == null || _levelText == null)
            return;

        int requireExp = req.exp;

        _levelText.text = $"Level {level}";
        _expSlider.maxValue = requireExp;
        _expSlider.value = exp;
        _expValue.text = $"{exp} / {requireExp}";
    }

    private void SetHpSlider(int hp, int maxHp)
    {
        if (_hpSlider == null || _hpValue == null)
            return;

        _hpSlider.maxValue = maxHp;
        _hpSlider.value = hp;
        _hpValue.text = $"{hp} / {maxHp}";
    }
}
