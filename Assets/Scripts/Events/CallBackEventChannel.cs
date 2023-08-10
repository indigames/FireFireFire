using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Events/CallBack Event Channel")]
public class CallBackEventChannel : ScriptableObject
{
    public Action<Action> OnEventRaised;

    public void RaiseEvent(Action action)
    {
        if (OnEventRaised == null)
        {
            Debug.LogWarning($"No one listen to this event {name}");
            return;
        }

        OnEventRaised.Invoke(action);
    }
}