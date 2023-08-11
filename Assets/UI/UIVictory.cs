using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static KantanManager;

public class UIVictory : MonoBehaviour
{
    [SerializeField] private GameObject GUI;

    public Gameplay gameplay;
    public Button btnNext;


    [Space]
    public Text txtScore;
    public Text txtRemainingBlockCount;
    public Text txtRemainingBlockBonusScore;

    [Header("Events")]
    public VoidEventChannel ShowVictoryAdsEnvent;
    public BoolEventChannel OnShowRewardAdsEvent;
    public VoidEventChannel gameEndEvent;
    public VoidEventChannel UITitleInspectEvent;

    [Space]
    [SerializeField] private VoidEventChannel PanelInspectEvent;
    [SerializeField] private CallBackEventChannel PanelHideEvent;

    [Header("Animation")]
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    AdMode adMode = AdMode.NoAd;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }
    private void OnEnable()
    {
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised += OnShowRewardAds;

        PanelHideEvent.OnEventRaised += OnPanelHideEvent;
        PanelInspectEvent.OnEventRaised += OnPanelInspectEvent;

        btnNext.onClick.AddListener(OnNext);
    }
    private void OnDisable()
    {
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised -= OnShowRewardAds;

        PanelHideEvent.OnEventRaised -= OnPanelHideEvent;
        PanelInspectEvent.OnEventRaised -= OnPanelInspectEvent;

        btnNext.onClick.RemoveAllListeners();
    }
    // private void Update()
    // {
    //     if (adMode == AdMode.NoAd)
    //     {
    //         KantanGameBox.ShowRewardAd();
    //         adMode = AdMode.ShowAd;
    //     }
    // }
    public void OnNext()
    {
        SendGameEndEvent();
        OnPanelHideEvent(ShowUiTitle);
    }
    public void ShowVictoryAds()
    {
        ShowVictoryAdsEnvent?.RaiseEvent();
    }
    public void OnShowRewardAds(bool isSuccess)
    {
        SendGameEndEvent();
        OnPanelHideEvent(ShowUiTitle);
    }

    private void ShowUiTitle()
    {
        UITitleInspectEvent.RaiseEvent();
    }

    public void OnAdsFail()
    {
        OnPanelHideEvent();
    }
    public void SendGameEndEvent()
    {
        gameEndEvent.RaiseEvent();
    }

    private void OnPanelHideEvent(System.Action callback = null)
    {
        System.Action totalCallBack = Hide;
        totalCallBack += callback;
        _fadeInFadeOutUI.FadeOut(totalCallBack);
    }
    private void Hide()
    {
        GUI.SetActive(false);
    }

    private void OnPanelInspectEvent()
    {
        Show();
        _fadeInFadeOutUI.FadeIn();
        Debug.Log("ShowVictory UI");
    }
    private void Show()
    {
        txtScore.text = "Score: " + (gameplay.CurrentStageScore + gameplay.RemainingBurnableObjectScore);
        txtRemainingBlockBonusScore.text = "+" + gameplay.RemainingBurnableObjectScore.ToString();
        txtRemainingBlockCount.text = gameplay.RemainingBurnableObjectCount.ToString();
        GUI.SetActive(true);
    }
}
