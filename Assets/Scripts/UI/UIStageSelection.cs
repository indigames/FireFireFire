using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageSelection : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    [SerializeField] private UIStageItemEntry[] stagesButton;
    [SerializeField] private Stage[] stages;

    [Header("Events")]
    [SerializeField] private StageEventChannel OnStageSelectEvent;
    [SerializeField] private VoidEventChannel PanelInspectEvent;

    [SerializeField] private VoidEventChannel OnFireShowEvent;
    [SerializeField] private VoidEventChannel OnFireHideEvent;

    [Header("Animations")]
    [SerializeField] private FadeInFadeOutUI _fadeInFadeOutUI;

    private void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        PanelInspectEvent.OnEventRaised += ShowPanelEvent;
        OnStageSelectEvent.OnEventRaised += OnStageSelectedReceived;

    }
    void OnDisable()
    {
        PanelInspectEvent.OnEventRaised -= ShowPanelEvent;
        OnStageSelectEvent.OnEventRaised -= OnStageSelectedReceived;
    }
    public void Init()
    {
        for (int i = 0; i < stagesButton.Length; i++)
        {
            stagesButton[i].Init(stages[i]);
        }
        HidePanel();
    }

    private void OnStageSelectedReceived(Stage stage)
    {
        HidePanelEvent();
    }

    public void HidePanelEvent()
    {
        _fadeInFadeOutUI.FadeOut(HidePanel);
    }

    public void HidePanel()
    {
        this.GUI.SetActive(false);
        OnFireHideEvent.RaiseEvent();
    }

    public void ShowPanelEvent()
    {
        ShowPanel();
        _fadeInFadeOutUI.FadeIn();
    }

    public void ShowPanel()
    {
        this.GUI.SetActive(true);
        OnFireShowEvent.RaiseEvent();
    }
}
