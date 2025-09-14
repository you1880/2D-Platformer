using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Activity : UI_Base
{
    private ISaveService _saveService;
    private IUserDataService _userDataService;

    private enum Images
    {
        CylinderGauge
    }

    private enum Texts
    {
        GaugeText
    }

    private Image _cylinderGauge;
    private RectTransform _gaugeRect;
    private TextMeshProUGUI _gaugeText;
    private float _height;
    private const string P1_GAUGE_COLOR = "#BDCCFF";
    private const string P2_GAUGE_COLOR = "#C4FFBD";
    private const string P3_GAUGE_COLOR = "#FFC7BD";

    public override void EnsureService()
    {
        _saveService = ServiceLocator.GetService<ISaveService>();
        _userDataService = ServiceLocator.GetService<IUserDataService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        InitGaugeStatus();
        BindEvent();
    }

    public override void Show() {}


    public override void Refresh()
    {
        InitGaugeStatus();
        BindEvent();
    }

    private void BindUIElements()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void GetUIElements()
    {
        _cylinderGauge = GetImage((int)Images.CylinderGauge);
        _gaugeRect = _cylinderGauge.GetComponent<RectTransform>();
        _gaugeText = GetText((int)Texts.GaugeText);
        _height = _gaugeRect.rect.height;
    }

    private void InitGaugeStatus()
    {
        Data.User.Userdata userData = _saveService.CurrentData;
        if (userData == null)
        {
            return;
        }

        string color = userData.playerType switch
        {
            1 => P1_GAUGE_COLOR,
            2 => P2_GAUGE_COLOR,
            3 => P3_GAUGE_COLOR,
            _ => P1_GAUGE_COLOR
        };

        _cylinderGauge.color = Util.GetColorFromHex(color);
        ChangeGaugeRatio(userData.activityPoint, userData.maxActivityPoint);
    }

    private void ChangeGaugeRatio(int cur, int max)
    {
        float gaugeRatio = (float)(max - cur) / max;
        float offsetMax = _height * gaugeRatio;

        _gaugeRect.offsetMax = new Vector2(0, -offsetMax);
        _gaugeText.text = $"{cur} / {max}";
    }

    private void BindEvent()
    {
        _userDataService.OnActivityChanged += ChangeGaugeRatio;
    }   

    private void UnbindEvent()
    {
        _userDataService.OnActivityChanged -= ChangeGaugeRatio;
    }

    private void OnDisable()
    {
        UnbindEvent();
    }
}
