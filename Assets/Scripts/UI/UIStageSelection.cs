using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageSelection : MonoBehaviour
{
    [SerializeField] private UIStageItemEntry[] stagesButton;
    [SerializeField] private Stage[] stages;

    [Header("Events")]
    [SerializeField] private StageEventChannel OnStageSelectEvent;

    private void Awake()
    {
        Init();
    }
    
    void OnEnable()
    {
        OnStageSelectEvent.OnEventRaised += OnStageSelectedReceived;
    }
    void OnDisable()
    {
        OnStageSelectEvent.OnEventRaised -= OnStageSelectedReceived;
    }
    public void Init()
    {
        for (int i = 0; i < stagesButton.Length; i++)
        {
            stagesButton[i].Init(stages[i]);
        }
    }

    private void OnStageSelectedReceived(Stage stage)
    {
        HidePanel();
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
}
