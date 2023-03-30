using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDefeat : MonoBehaviour
{
    public Gameplay gameplay;
    public Animator anim;
    public GameObject retryBtn;
    public GameObject adBtn;
    public VoidEventChannel ShowLostAdsEnvent;
    public BoolEventChannel OnShowRewardAdsEvent;

    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackDefeat += () => gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        retryBtn.SetActive(false);
        StartCoroutine(Show());
          if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised += OnShowRewardAds;
    }
    private void OnDisable() {
         if (OnShowRewardAdsEvent != null)
            OnShowRewardAdsEvent.OnEventRaised -= OnShowRewardAds;
    }
    public void Continue()
    {
        gameplay.RestartGame(false, false);
        StartCoroutine(Hide());
    }
    public void ShowLostAds()
    {
        ShowLostAdsEnvent?.RaiseEvent();
    }
    public void OnShowRewardAds(bool isSuccess)
    {
        gameplay.RestartGame(false, isSuccess);
        StartCoroutine(Hide());
    }
    IEnumerator Hide()
    {
        yield return true;
        anim.Play("Hide");
    }
    IEnumerator Show()
    {
        yield return new WaitForSeconds(4);
        retryBtn.SetActive(true);
    }
}
