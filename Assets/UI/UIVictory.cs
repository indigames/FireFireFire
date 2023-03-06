using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVictory : MonoBehaviour
{
    public Gameplay gameplay;
    public Animator anim;
    public GameObject nextBtn;
    // public GameObject adBtn;
    public VoidEventChannel ShowVictoryAdsEnvent;
    public BoolEventChannel OnShowRewardAdsEvent;
        public IntEventChannel gameEndEvent;
    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackVictory += () => gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (OnShowRewardAdsEvent != null) OnShowRewardAdsEvent.OnEventRaised += OnShowRewardAds;
        nextBtn.SetActive(false);
        StartCoroutine(Show());
    }
    private void OnDisable()
    {
        if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised -= OnShowRewardAds;
    }
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
        gameplay.RestartGame(true, isSuccess);
        StartCoroutine(Hide());
    }
    public void SendGameEndEvent(int score)
    {
        Debug.Log("SendGameEndEvent");
        gameEndEvent?.RaiseEvent(score);
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
