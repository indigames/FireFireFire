using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] private GameObject GUI;
    public Gameplay gameplay;

    [Space]
    public GameObject itemPreview;
    public GameObject grabIcon;
    public Text textRemaining;
    public Text textStage;
    public string textStageFormat = "Stage {0}";

    [SerializeField] private Text _txtScore;
    [SerializeField] private float SCORE_UPDATE_DURATION = 0.2f;

    [SerializeField] private Button _btnPause;

    [Header("Event")]
    [SerializeField] private IntEventChannel OnScoreChangeEvent;

    [SerializeField] private VoidEventChannel OnPanelInspect;
    [SerializeField] private VoidEventChannel OnPanelHide;

    [Space]
    [SerializeField] private VoidEventChannel OnUIPauseInspect;

    private float _lastScore;

    void OnEnable()
    {
        OnPanelInspect.OnEventRaised += ShowPanel;
        OnPanelHide.OnEventRaised += HidePanel;
        OnScoreChangeEvent.OnEventRaised += OnScoreChange;

        _btnPause.onClick.AddListener(OnPause);
    }

    void OnDisable()
    {
        OnPanelInspect.OnEventRaised -= ShowPanel;
        OnPanelHide.OnEventRaised -= HidePanel;
        OnScoreChangeEvent.OnEventRaised -= OnScoreChange;

        _btnPause.onClick.RemoveAllListeners();
    }
    private void OnPause()
    {
        OnUIPauseInspect.RaiseEvent();
    }

    private void OnScoreChange(int value)
    {
        StartCoroutine(IncreaseScore(value));
    }

    private IEnumerator IncreaseScore(float currentScore)
    {
        float elapsed = 0;
        float addedScore = currentScore - _lastScore;
        while (elapsed <= SCORE_UPDATE_DURATION)
        {
            elapsed += Time.unscaledDeltaTime;
            float timeRatio = elapsed / SCORE_UPDATE_DURATION;
            float tmpScore = (int)(addedScore * timeRatio) + _lastScore;
            _txtScore.text = tmpScore.ToString();
            yield return null;
        }
        _lastScore = currentScore;
        _txtScore.text = _lastScore.ToString("##");
    }

    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackRestart += Restart;
        gameplay.callbackRemainingMeshblock += ResetRemainingCount;
        HidePanel();
    }

    private void Update()
    {
        if (gameplay.DraggingMeshBlock == null && grabIcon.activeSelf) grabIcon.SetActive(false);
        if (gameplay.DraggingMeshBlock != null && grabIcon.activeSelf == false) grabIcon.SetActive(true);

        if (gameplay.DraggingMeshBlock != null)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(gameplay.DraggingMeshBlock.transform.position);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)grabIcon.transform.parent, screenPoint, Camera.main, out localPoint);
            //var pos = grabIcon.transform.localPosition;
            //pos.z = 0;
            grabIcon.transform.localPosition = localPoint;
        }
    }

    void Restart()
    {
        textStage.text = string.Format(textStageFormat, gameplay.currentStage.IDNumber);
        textRemaining.text = "";
        _lastScore = 0;
        _txtScore.text = "0";
        StartCoroutine(DelayPreview());
    }

    IEnumerator DelayPreview()
    {
        itemPreview.SetActive(false);
        yield return true;
        yield return true;
        itemPreview.SetActive(true);
    }

    void ResetRemainingCount(int count)
    {

        textRemaining.text = string.Format("x<color=red>{0}</color>", count);


        //if (count > 1)
        //    textRemaining.text = string.Format("<color=red>{0}</color> items remaning", count);
        //else if (count == 1)
        //    textRemaining.text = "<color=red>1</color> item remaning";
        //else
        //    textRemaining.text = "No item remaning";
    }

    private void ShowPanel()
    {
        GUI.SetActive(true);
    }

    private void HidePanel()
    {
        GUI.SetActive(false);
    }
}
