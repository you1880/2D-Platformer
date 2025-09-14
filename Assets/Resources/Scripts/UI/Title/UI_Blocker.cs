using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Blocker : UI_Base
{
    public override void EnsureService()
    {
    }

    public override void Init()
    {
    }

    public override void Show() {}

    
    private void OnEnable()
    {
        gameObject.transform.SetAsLastSibling();
    }
}
