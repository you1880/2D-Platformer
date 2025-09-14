using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] Button _button;
    private ISoundService _soundService;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => ServiceLocator.IsInitialized);

        _soundService = ServiceLocator.GetService<ISoundService>();
        _button.onClick.AddListener(() => _soundService.SetEffectVolume(0.5f));
    }
}
