using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    [Space]
    [SerializeField] private Button _btnContinue;
    [SerializeField] private Button _btnRematch;
    [SerializeField] private Button _btnRetire;

    [Header("Events")]
    [SerializeField] private VoidEventChannel _onPanelInspectEvent;
    [SerializeField] private CallBackEventChannel _onPanelHideEvent;

    [SerializeField] private VoidEventChannel _onGamePlayRetryEvent;
    [SerializeField] private VoidEventChannel _onGamePlayRetireEvent;

    [SerializeField] private VoidEventChannel _onUIStageSelectInspect;
    [SerializeField] private VoidEventChannel _onUIGamePlayHide;

    [Header("Animations")]
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    void Start()
    {
        HidePanel();
    }

    private void OnEnable()
    {
        _onPanelInspectEvent.OnEventRaised += OnShowPanelEvent;
        _onPanelHideEvent.OnEventRaised += OnHidePanelEvent;

        _btnContinue.onClick.AddListener(OnContinue);
        _btnRematch.onClick.AddListener(OnRematch);
        _btnRetire.onClick.AddListener(OnRetire);
    }

    private void OnDisable()
    {
        _onPanelInspectEvent.OnEventRaised -= OnShowPanelEvent;
        _onPanelHideEvent.OnEventRaised -= OnHidePanelEvent;

        _btnContinue.onClick.RemoveAllListeners();
        _btnRematch.onClick.RemoveAllListeners();
        _btnRetire.onClick.RemoveAllListeners();
    }
    private void OnContinue()
    {
        OnHidePanelEvent();
    }
    private void OnRematch()
    {
        OnHidePanelEvent(_onGamePlayRetryEvent.RaiseEvent);
    }
    private void OnRetire()
    {
        Action callback = _onGamePlayRetireEvent.RaiseEvent;
        callback += _onUIStageSelectInspect.RaiseEvent;
        callback += _onUIGamePlayHide.RaiseEvent;
        OnHidePanelEvent(callback);
    }

    private void OnShowPanelEvent()
    {
        ShowPanel();
        _fadeInFadeOutUI.FadeIn();
    }
    private void ShowPanel()
    {
        Time.timeScale = 0;
        this.GUI.SetActive(true);
    }

    private void OnHidePanelEvent(Action callback = null)
    {
        Action totalCallBack = HidePanel;
        totalCallBack += callback;
        _fadeInFadeOutUI.FadeOut(totalCallBack);
    }
    private void HidePanel()
    {
        Time.timeScale = 1;
        this.GUI.SetActive(false);
    }
}
