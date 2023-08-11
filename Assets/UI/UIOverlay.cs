using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    [SerializeField] private CallBackEventChannel _ShowPanelEvent;
    [SerializeField] private CallBackEventChannel _HidePanelEvent;

    bool firstTime = true;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        HidePanel();
    }

    private void OnEnable()
    {
        _ShowPanelEvent.OnEventRaised += OnShowPanelReceived;
        _HidePanelEvent.OnEventRaised += OnHidePanelReceived;

    }
    void OnDisable()
    {
        _ShowPanelEvent.OnEventRaised -= OnShowPanelReceived;
        _HidePanelEvent.OnEventRaised -= OnHidePanelReceived;
    }

    private void OnShowPanelReceived(Action callback)
    {
        ShowPanel();
        _fadeInFadeOutUI.FadeIn(callback);
    }

    private void OnHidePanelReceived(Action callback)
    {
        Action totalCallBack = HidePanel;
        totalCallBack += callback;
        _fadeInFadeOutUI.FadeOut(totalCallBack);
    }

    private void OnShowPanelEvent()
    {
        if (firstTime)
        {
            firstTime = false;
            return;
        }
        ShowPanel();
        _fadeInFadeOutUI.FadeIn();
    }
    void ShowPanel()
    {
        GUI.SetActive(true);
    }
    private void OnHidePanelEvent()
    {
        _fadeInFadeOutUI.FadeOut(HidePanel);
    }
    void HidePanel()
    {
        GUI.SetActive(false);
    }
}
