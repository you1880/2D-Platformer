using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[AutoRegister(typeof(ISceneService), priority: 15)]
public class SceneService : ISceneService
{
    private IUIService _uiService;

    public SceneService(IUIService uiService)
    {
        _uiService = uiService;
    }

    private const float WAIT_TIME = 1.0f;
    private const float FADE_DELAY_TIME = 1.0f;
    private const float ALPHA_OPAQUE = 1.0f;
    private const float ALPHA_TRANSPARENT = 0.0f;
    private GameObject _blocker;
    private UI_Blocker _blockerUI;
    private Image _blockerImage;

    public BaseScene CurrentScene => GameObject.Find("@Scene")?.GetComponent<BaseScene>();
    public Define.SceneType CurrentSceneType => CurrentScene == null ? Define.SceneType.Unknown : CurrentScene.SceneType;

    public void LoadScene(Define.SceneType sceneType)
    {
        if (CurrentSceneType == sceneType)
        {
            return;
        }

        string sceneName = GetSceneName(sceneType);
        CoroutineHandler.Instance.RunCoroutine(LoadSceneAsync(sceneName));
    }

    private string GetSceneName(Define.SceneType sceneType) => Enum.GetName(typeof(Define.SceneType), sceneType);

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float startTime = Time.time;

        yield return CoroutineHandler.Instance.RunCoroutine(FadeOut());
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        float elapsed = Time.time - startTime;

        if (elapsed < WAIT_TIME)
        {
            yield return new WaitForSeconds(WAIT_TIME - elapsed);
        }

        yield return new WaitForSeconds(WAIT_TIME / 2.0f);

        operation.allowSceneActivation = true;
        _uiService.ClearUIDictionary();

        yield return null;
        yield return CoroutineHandler.Instance.RunCoroutine(FadeIn());
    }
    
    private void CreateBlockerUI()
    {
        _blockerUI = _uiService.ShowUI<UI_Blocker>();
        _blocker = _blockerUI.gameObject;
        _blockerImage = _blocker.GetComponent<Image>();
    }

    private IEnumerator Fade(float start, float end)
    {
        CreateBlockerUI();

        float elapsed = 0.0f;

        Color color = _blockerImage.color;
        _blockerImage.color = new Color(color.r, color.g, color.b, start);

        while (elapsed < FADE_DELAY_TIME)
        {
            if (_blockerImage == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / FADE_DELAY_TIME);
            float alpha = Mathf.Lerp(start, end, t);

            _blockerImage.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        yield return null;
    }

    private IEnumerator FadeIn()
    {
        yield return CoroutineHandler.Instance.RunCoroutine(Fade(ALPHA_OPAQUE, ALPHA_TRANSPARENT));

        _uiService.CloseUI(_blocker);
    }

    private IEnumerator FadeOut()
    {
        yield return CoroutineHandler.Instance.RunCoroutine(Fade(ALPHA_TRANSPARENT, ALPHA_OPAQUE));
    }
}
