using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOutUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _fadeInSpeed;
    [SerializeField] private float _fadeInDelay;
    [SerializeField] private float _fadeOutSpeed;
    [SerializeField] private float _fadeOutDelay;
    public System.Action _callback;

    public void FadeIn(System.Action callback = null)
    {
        this._callback = callback;
        StartCoroutine(CoFadeIn());
    }

    public void FadeOut(System.Action callback = null)
    {
        this._callback = callback;
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeIn()
    {
        
        _canvasGroup.alpha = 0;
        yield return new WaitForSeconds(_fadeInDelay);
        while (true)
        {
            yield return null;
            if (!CalculateAndCheckCanvasGroupValue(1)) break;
        }
        _callback?.Invoke();
    }

    IEnumerator CoFadeOut()
    {
        _canvasGroup.alpha = 1;
        yield return new WaitForSeconds(_fadeOutDelay);
        while (true)
        {
            yield return null;
            if (!CalculateAndCheckCanvasGroupValue(0)) break;
        }
        _callback?.Invoke();
    }

    private bool CalculateAndCheckCanvasGroupValue(float output)
    {
        if (_canvasGroup.alpha < output)
        {
            _canvasGroup.alpha +=_fadeInSpeed * Time.deltaTime ;
            if (_canvasGroup.alpha >= output) return false;
        }
        else
        {
            _canvasGroup.alpha -= _fadeOutSpeed * Time.deltaTime;
            if (_canvasGroup.alpha <= output) return false;
        }
        return true;
    }
}
