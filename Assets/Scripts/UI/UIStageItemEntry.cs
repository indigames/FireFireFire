using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageItemEntry : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Stage currentStage;
    [Header("Events")]
    [SerializeField] private StageEventChannel OnStageSelectEvent;
    public void Init(Stage stage)
    {
        currentStage = stage;
        button.onClick.AddListener(OnStageSelect);
    }

    public void OnStageSelect()
    {
        OnStageSelectEvent.RaiseEvent(currentStage);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
