using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Animation : MonoBehaviour
{
    private Vector3 _expandedScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _contractScale = new Vector3(0.0f, 0.0f, 0.0f);
    private const float SCALE_DURATION = 0.5f;
    private const float FADE_TIME = 1.0f;

    #region Scale In / Out
    public IEnumerator ShowUIScaleIn(RectTransform rectTransform)
    {
        float elapsedTime = 0.0f;
        rectTransform.localScale = _contractScale;

        while (elapsedTime < SCALE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(_contractScale, _expandedScale, elapsedTime / SCALE_DURATION);

            yield return null;
        }

        rectTransform.localScale = _expandedScale;
    }

    public IEnumerator ShowUIScaleOut(RectTransform rectTransform)
    {
        float elapsedTime = 0.0f;
        rectTransform.localScale = _expandedScale;

        while (elapsedTime < SCALE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(_expandedScale, _contractScale, elapsedTime / SCALE_DURATION);

            yield return null;
        }

        rectTransform.localScale = _contractScale;
    }
    #endregion

    #region Fade In / Out
    public IEnumerator FadeIn(Image image)
    {
        if (image == null)
        {
            yield break;
        }

        float elapsedTime = 0.0f;
        Color originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);

        while (elapsedTime < SCALE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / SCALE_DURATION);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);

        yield return null;
    }

    public IEnumerator FadeOut(Image image)
    {
        if (image == null)
        {
            yield break;
        }

        float elapsedTime = 0.0f;
        Color originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);

        while (elapsedTime < SCALE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / SCALE_DURATION);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);

        yield return null;
    }
    #endregion

    #region BlinkText
    public IEnumerator BlinkText(TextMeshProUGUI text, float duration = 0.5f, float minAlpha = 0.0f, float maxAlpha = 1.0f)
    {
        if (text == null)
        {
            yield break;
        }
        Color original = text.color;
        float elapsedTime = 0.0f;

        while (true)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(elapsedTime / duration, 1.0f));

            text.color = new Color(original.r, original.g, original.b, alpha);

            yield return null;
        }
    }

    public void StopBlinkingText(TextMeshProUGUI text)
    {
        if (text == null)
        {
            Debug.Log("Text is null");
            return;
        }

        Color original = text.color;
        text.color = new Color(original.r, original.g, original.b, 1.0f);
    }
    #endregion

    #region Move Up / Down
    public IEnumerator MoveUp(RectTransform rectTransform, float targetYPos, float duration)
    {
        if (rectTransform == null)
        {
            yield break;
        }

        Vector3 startPosition = rectTransform.anchoredPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, targetYPos, 0);
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    public IEnumerator MoveDown(RectTransform rectTransform, float originalYPos, float duration)
    {
        if (rectTransform == null)
        {
            yield break;
        }

        Vector3 startPosition = rectTransform.anchoredPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, originalYPos, 0);
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
    #endregion
}
