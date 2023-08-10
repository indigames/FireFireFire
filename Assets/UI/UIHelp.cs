using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelp : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    [SerializeField] private GameObject TutorialModel;
    [SerializeField] private GameObject FirstTutorial;
    [SerializeField] private GameObject SecondTutorial;
    [SerializeField] private Button btnNext;

    [Header("Events")]
    [SerializeField] private VoidEventChannel OnPanelInspectEvent;
    [SerializeField] private VoidEventChannel OnPanelHideEvent;

    [Space]
    [SerializeField] private VoidEventChannel UIGamePlayInspect;
    [SerializeField] private VoidEventChannel UIGamePlayHide;

    [Header("Animation")]
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    private void OnEnable()
    {
        OnPanelInspectEvent.OnEventRaised += OnShowPanelEvent;
    }

    void OnDisable()
    {
        OnPanelInspectEvent.OnEventRaised -= OnShowPanelEvent;
    }


    private void ShowFirstTutorial()
    {
        FirstTutorial.SetActive(true);

        btnNext.onClick.RemoveAllListeners();
        btnNext.onClick.AddListener(ShowSecondTutorial);
    }

    public void ShowSecondTutorial()
    {
        FirstTutorial.SetActive(false);
        SecondTutorial.SetActive(true);


        btnNext.onClick.RemoveAllListeners();
        btnNext.onClick.AddListener(OnHidePanelEvent);
    }

    private void OnShowPanelEvent()
    {
        ShowPanel();
        _fadeInFadeOutUI.FadeIn();
    }

    private void OnHidePanelEvent()
    {
        _fadeInFadeOutUI.FadeOut(HidePanel);
    }

    private void ShowPanel()
    {
        GUI.SetActive(true);
        ShowFirstTutorial();
        TutorialModel.SetActive(true);
        UIGamePlayInspect.RaiseEvent();
    }

    public void HidePanel()
    {
        GUI.SetActive(false);
        FirstTutorial.SetActive(false);
        SecondTutorial.SetActive(false);
        TutorialModel.SetActive(false);
        OnPanelHideEvent.RaiseEvent();

        btnNext.onClick.RemoveAllListeners();

        UIGamePlayHide.RaiseEvent();
    }
}
