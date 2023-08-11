using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDefeat : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    public Gameplay gameplay;
    public Animator anim;
    public Button retryBtn;
    public GameObject adBtn;
    public Text txtScore;
    public VoidEventChannel ShowLostAdsEnvent;
    public BoolEventChannel OnShowRewardAdsEvent;

    [SerializeField] private VoidEventChannel OnUIDefeatInspect;
    [SerializeField] private CallBackEventChannel OnUIDefeatHide;

    [SerializeField] private VoidEventChannel OnUITitleInspect;

    [Header("Animations")]
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    // Start is called before the first frame update
    void Start()
    {
        HidePanel();
    }
    private void OnEnable()
    {
        ShowPanel();
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised += OnShowRewardAds;

        OnUIDefeatInspect.OnEventRaised += OnShowPanelEvent;
        OnUIDefeatHide.OnEventRaised += OnHidePanelEvent;
    }
    private void OnDisable()
    {
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised -= OnShowRewardAds;
        OnUIDefeatInspect.OnEventRaised -= OnShowPanelEvent;
        OnUIDefeatHide.OnEventRaised -= OnHidePanelEvent;
    }
    public void Continue()
    {
        OnHidePanelEvent(OnUITitleInspect.RaiseEvent);
    }
    public void ShowLostAds()
    {
        ShowLostAdsEnvent?.RaiseEvent();
    }
    public void OnShowRewardAds(bool isSuccess)
    {
        OnHidePanelEvent();
    }
    private void OnHidePanelEvent(Action callback = null)
    {
        Action totalCallback = HidePanel;
        totalCallback += callback;
        _fadeInFadeOutUI.FadeOut(totalCallback);
    }
    private void HidePanel()
    {
        this.GUI.SetActive(false);
    }

    private void OnShowPanelEvent()
    {
        ShowPanel();
        _fadeInFadeOutUI.FadeIn();
    }
    private void ShowPanel()
    {
        txtScore.text = "Score: " + gameplay.CurrentStageScore;
        this.GUI.SetActive(true);
    }
}
