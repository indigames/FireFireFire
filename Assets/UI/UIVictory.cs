using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KantanManager;

public class UIVictory : MonoBehaviour
{
    public Gameplay gameplay;
    public Animator anim;
    public GameObject nextBtn;
    // public GameObject adBtn;
    public VoidEventChannel ShowVictoryAdsEnvent;
    public BoolEventChannel OnShowRewardAdsEvent;
    public VoidEventChannel gameEndEvent;
    AdMode adMode = AdMode.NoAd;
    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackVictory += () => gameObject.SetActive(true);
        gameObject.SetActive(false);

    }
    private void OnEnable()
    {
        nextBtn.SetActive(false);
        StartCoroutine(Show());
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised += OnShowRewardAds;
    }
    private void OnDisable()
    {
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised -= OnShowRewardAds;
    }
    // private void Update()
    // {
    //     if (adMode == AdMode.NoAd)
    //     {
    //         KantanGameBox.ShowRewardAd();
    //         adMode = AdMode.ShowAd;
    //     }
    // }
    public void Continue()
    {
        gameplay.RestartGame(true, false);
        StartCoroutine(Hide());
    }
    public void ShowVictoryAds()
    {
        ShowVictoryAdsEnvent?.RaiseEvent();
    }
    public void OnShowRewardAds(bool isSuccess)
    {
        SendGameEndEvent();
        gameplay.RestartGame(true, true);
        StartCoroutine(Hide());
    }
    
    public void OnAdsFail()
    {
        gameplay.RestartGame(true, false);
        StartCoroutine(Hide());
    }
    public void SendGameEndEvent()
    {
        gameEndEvent.RaiseEvent();
    }
    IEnumerator Hide()
    {
        yield return true;
        anim.Play("Hide");
    }
    IEnumerator Show()
    {
        yield return new WaitForSeconds(4);
        nextBtn.SetActive(true);
    }
}
