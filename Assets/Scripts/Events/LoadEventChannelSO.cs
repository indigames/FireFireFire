using UnityEngine;
using UnityEngine.Events;
using System;


[CreateAssetMenu(menuName = "Events/Load Event Channel")]
public class LoadEventChannelSO : ScriptableObject
{
    public UnityAction<GameSceneSO> OnLoadSceneRequested;

    public void RequestLoadScene(GameSceneSO sceneSO)
    {
        if (OnLoadSceneRequested != null)
            OnLoadSceneRequested.Invoke(sceneSO);
        else
            Debug.LogWarning("A scene loading was requested, but no one picking this up. " +
                             "SceneLoader should be presented and listen to this event.");
    }
}