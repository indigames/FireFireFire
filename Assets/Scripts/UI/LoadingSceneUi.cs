using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUi : MonoBehaviour
{
    #region Variables

    [SerializeField] private Slider loadingBar;
    [SerializeField] private Text loadingPercent;

    [Header("Listen Events")] [SerializeField]
    private FloatEventChannel OnProgress;

    #endregion

    #region Unity Methods

    private void Start()
    {
        loadingPercent.text = "0%";
        loadingBar.value = 0;
    }

    private void OnEnable()
    {
        OnProgress.OnEventRaised += UpdateProgressBar;
    }

    private void OnDisable()
    {
        OnProgress.OnEventRaised -= UpdateProgressBar;
    }

    private void UpdateProgressBar(float progress)
    {
        loadingPercent.text = ((int)progress * 100).ToString() + " %";
        Debug.Log($"Progress: {progress * 100}");
        loadingBar.normalizedValue = progress;
    }

    #endregion
}