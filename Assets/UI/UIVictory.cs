using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static KantanManager;

public class UIVictory : MonoBehaviour
{
    public Gameplay gameplay;
    public Animator anim;
    public GameObject nextBtn;
    // public GameObject adBtn;
    public Text txtScore;
    

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
        SendGameEndEvent();
        StartCoroutine(Hide());
    }
    public void ShowVictoryAds()
    {
        ShowVictoryAdsEnvent?.RaiseEvent();
    }
    public void OnShowRewardAds(bool isSuccess)
    {
        SendGameEndEvent();
        StartCoroutine(Hide());
    }
    
    public void OnAdsFail()
    {
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
        txtScore.text = "Score: " + gameplay.CurrentStageScore;
        yield return null;
        nextBtn.SetActive(true);
    }
}
