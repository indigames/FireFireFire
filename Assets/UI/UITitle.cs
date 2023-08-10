using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    [SerializeField] private Button btnHelp;
    [SerializeField] private Button btnStartGame;

    [Header("Events")]
    [SerializeField] private VoidEventChannel PanelInspectEvent;
    [SerializeField] private VoidEventChannel PanelHideEvent;
    [Space]
    [SerializeField] private VoidEventChannel OnPanelHideEvent;
    [SerializeField] private VoidEventChannel OnUIHelpInspectEvent;

    [SerializeField] private VoidEventChannel OnFireShowEvent;
    [SerializeField] private VoidEventChannel OnFireHideEvent;

    [Header("Animation")]
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    void Start()
    {
        ShowPanelEvent();
    }

    private void OnEnable()
    {
        PanelInspectEvent.OnEventRaised += ShowPanelEvent;
        PanelHideEvent.OnEventRaised += HidePanelEvent;

        btnStartGame.onClick.AddListener(OnStartGameButtonPressed);
        btnHelp.onClick.AddListener(OnHelpButtonPressed);
    }

    private void OnHelpButtonPressed()
    {
        OnUIHelpInspectEvent.RaiseEvent();
        HidePanel();
    }

    private void OnDisable()
    {
        PanelInspectEvent.OnEventRaised -= ShowPanel;
        PanelHideEvent.OnEventRaised -= HidePanel;

        btnStartGame.onClick.RemoveAllListeners();
        btnHelp.onClick.RemoveAllListeners();
    }
    private void OnStartGameButtonPressed()
    {
        _fadeInFadeOutUI.FadeOut(HidePanelWithCallback);
    }

    private void HidePanelWithCallback()
    {
        HidePanel();
        OnPanelHideEvent.RaiseEvent();
    }

    private void ShowPanelEvent()
    {
        ShowPanel();
        _fadeInFadeOutUI.FadeIn(null);
    }

    private void HidePanelEvent()
    {
        _fadeInFadeOutUI.FadeOut(HidePanel);
    }

    private void ShowPanel()
    {
        GUI.SetActive(true);
        OnFireShowEvent.RaiseEvent();
    }

    private void HidePanel()
    {
        GUI.SetActive(false);
        OnFireHideEvent.RaiseEvent();
    }

}
