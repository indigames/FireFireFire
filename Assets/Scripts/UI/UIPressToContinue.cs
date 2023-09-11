using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPressToContinue : MonoBehaviour
{
    [SerializeField] private GameObject GUI;

    [SerializeField] private Button btnPress;

    void OnEnable()
    {
        btnPress.onClick.AddListener(OnFocus);
    }

    void OnDisable()
    {
        btnPress.onClick.RemoveAllListeners();
    }

    public void OnUnFocus()
    {
        StopAudio();
        Show();

        Time.timeScale = 0;
    }

    public void OnFocus()
    {
        UnPauseAudio();
        Hide();

        Time.timeScale = 1;
    }

    private void StopAudio()
    {
        var audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var item in audioSources)
        {
            item.Pause();
        }
    }

    private void UnPauseAudio()
    {
        var audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var item in audioSources)
        {
            item.UnPause();
        }
    }

    public void Show()
    {
        GUI.SetActive(true);
    }

    public void Hide()
    {
        GUI.SetActive(false);
    }
}
